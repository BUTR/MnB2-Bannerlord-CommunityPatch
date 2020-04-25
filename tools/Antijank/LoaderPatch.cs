using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;
using Medallion.Collections;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.TwoDimension.Standalone;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;
using Path = System.IO.Path;

namespace Antijank {

  public struct WeightedModuleInfo {

    public ModuleInfo Module;

    public int Weight;

    public WeightedModuleInfo(ModuleInfo mod, int weight) {
      Module = mod;
      Weight = weight;
    }

  }

  public static class LoaderPatch {

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    static LoaderPatch() {
      Context.Harmony.Patch(
        typeof(LauncherModsVM).GetMethod("LoadSubModules", Declared),
        new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(LoadSubModulesPrefix), Declared)));

      Context.Harmony.Patch(
        typeof(LauncherModsVM).GetMethod("ChangeLoadingOrderOf", Declared),
        new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(ChangeLoadingOrderOfPrefix), Declared)));

      Context.Harmony.Patch(
        typeof(LauncherUI).GetConstructors(Declared).First(),
        postfix: new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(LauncherUICtorPostfix), Declared)));

      Context.Harmony.Patch(
        typeof(GraphicsForm).GetMethod("UpdateInput", Declared),
        new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(GraphicsFormUpdateInputPatch))));

      Context.Harmony.Patch(
        typeof(GraphicsForm).GetMethod("MessageHandler", Declared),
        new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(GraphicsFormMessageHandlerPatch))));
    }

    public static void Init() {
      // static initializer

      for (var i = 1; i <= 255; ++i) {
        var name = Enum.GetName(typeof(InputKey), (InputKey) i);
        if (Enum.TryParse(name, out VirtualKeyCode vk) && (int) vk <= 255)
          VirtualKeyCodeToInputKeyMap[(byte) vk] = (byte) i;
      }
    }

    private static LauncherUI LauncherUI;

    private static UIContext UIContext;

    private static AccessTools.FieldRef<LauncherUI, LauncherVM> _launcherVmAccessor;

    private static readonly byte[] VirtualKeyCodeToInputKeyMap = new byte[256];

    private static AccessTools.FieldRef<LauncherUI, GauntletMovie> _gauntletMovieAccessor;

    [DllImport("user32")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    public static unsafe bool GraphicsFormMessageHandlerPatch(WindowMessage message, ref long wParam, long lParam) {
      // NOTE: f10, left and right alt are a lost cause here

      if (message != WindowMessage.KeyDown && message != WindowMessage.KeyUp)
        return true;

      switch (wParam) {
        case 0x10: // VK_SHIFT
          wParam = MapVirtualKey((uint) (lParam >> 16) & 0xFF, /*MAPVK_VSC_TO_VK_EX*/ 3);
          break;
        case 0x11: // VK_CONTROL
          wParam = (long) (0 != ((lParam >> 24) & 1) ? VirtualKeyCode.LeftControl : VirtualKeyCode.RightControl);
          break;
      }

      wParam = wParam <= VirtualKeyCodeToInputKeyMap.Length
        ? VirtualKeyCodeToInputKeyMap[(byte) wParam]
        : 0;

      return true;
    }

    public static unsafe bool GraphicsFormUpdateInputPatch(
      StandaloneUIDomain __instance,
      ref bool ____mouseOverDragArea,
      ref InputData ____oldInputData,
      ref InputData ____currentInputData,
      ref object ____inputDataLocker,
      ref InputData ____messageLoopInputData,
      bool mouseOverDragArea) {
      Volatile.Write(ref ____mouseOverDragArea, mouseOverDragArea);
      lock (____inputDataLocker) {
        Interlocked.Exchange(
          ref ____messageLoopInputData, // = ____oldInputData (third)
          Interlocked.Exchange(
            ref ____oldInputData, // = ____currentInputData (second)
            Interlocked.Exchange(
              ref ____currentInputData, // = ____messageLoopInputData (first)
              ____messageLoopInputData
            )
          )
        );

        ____messageLoopInputData.CursorX = ____currentInputData.CursorX;
        ____messageLoopInputData.CursorY = ____currentInputData.CursorY;
        ____messageLoopInputData.LeftMouse = ____currentInputData.LeftMouse;
        ____messageLoopInputData.RightMouse = ____currentInputData.RightMouse;
        ____messageLoopInputData.MouseMove = ____currentInputData.MouseMove;
        ____messageLoopInputData.MouseScrollDelta = default;

        fixed (bool* pSrc = ____currentInputData.KeyData)
        fixed (bool* pDest = ____messageLoopInputData.KeyData)
          Unsafe.CopyBlock(pDest, pSrc, 256);
      }

      return false;
    }

    private static bool waitForSortKeysReset = false;

    public static void LauncherUICtorPostfix(LauncherUI __instance, UserDataManager userDataManager, UIContext context, Action onClose, Action onMinimize) {
      LauncherUI = __instance;
      UIContext = context;
      _launcherVmAccessor = AccessTools.FieldRefAccess<LauncherUI, LauncherVM>("_viewModel");
      _gauntletMovieAccessor = AccessTools.FieldRefAccess<LauncherUI, GauntletMovie>("_movie");

      var input = context.EventManager.InputContext;

      void KeyWatcher(float tick) {
        SynchronizationContext.Current.Post(_ => {
          context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5);
        }, null);

        var ctrl = input.IsControlDown();
        var s = input.IsKeyDown(InputKey.S);

        if (waitForSortKeysReset) {
          if (!ctrl || !s)
            waitForSortKeysReset = false;
          return;
        }

        var wantsToSort = ctrl && s;
        if (!wantsToSort)
          return;

        waitForSortKeysReset = true;
        Console.WriteLine("Sorting modules list.");
        ref var launcherVm = ref _launcherVmAccessor(__instance);
        var isMultiplayer = launcherVm.IsMultiplayer;

        var dict = ModuleList.ToDictionary(mi => mi.Id);

        var list = ModuleList
          .OrderTopologicallyBy(mi => mi.DependedModuleIds.Select(id => dict[id]))
          .ThenBy(mi => mi.IsNative() ? 0 : 1)
          .ThenBy(mi => mi.Alias)
          .ToList();

        IdentitySort(launcherVm.ModsData.Modules, list);

        _gauntletMovieAccessor(__instance)
          .RefreshDataSource(launcherVm);
      }

      context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5);
    }

    public static bool LoadSubModulesPrefix(LauncherModsVM __instance, UserData ____userData, bool isMultiplayer) {
      var list = GetOrderedModuleList(____userData, isMultiplayer);

      ModuleList = new MBReadOnlyList<ModuleInfo>(list);

      foreach (var module in ModuleList) {
        UnblockModule(module);
        var launcherModuleVm = new LauncherModuleVM(module,
          // NOTE: this doesn't actually get used
          (targetModule, insertIndex, tag) => ChangeLoadingOrderOf(__instance, targetModule, insertIndex, tag),
          targetModule => ChangeIsSelectedOf(__instance, ____userData, isMultiplayer, targetModule));
        __instance.Modules.Add(launcherModuleVm);
      }

      return false;
    }

    private static List<ModuleInfo> GetOrderedModuleList(UserData userData, bool isMultiplayer) {
      var data = isMultiplayer
        ? userData.MultiplayerData
        : userData.SingleplayerData;

      var modInfos = ModuleInfo.GetModules()
        .Where(mod => IsVisible(isMultiplayer, mod))
        .Select(mod => {
          var userModData = userData.GetUserModData(isMultiplayer, mod.Id);
          if (userModData != null)
            mod.IsSelected = mod.IsSelected || userModData.IsSelected;
          var id = mod.Id;
          var pref = data.ModDatas.FindIndex(md => md.Id == id);
          if (pref < 0) pref = int.MinValue;
          return new WeightedModuleInfo(mod, pref);
        })
        .ToList();

      var dict = modInfos.ToDictionary(mi => mi.Module.Id);

      var list = modInfos
        .OrderTopologicallyBy(mi => mi.Module.DependedModuleIds.Select(id => dict[id]))
        .ThenBy(mi => mi.Module.IsNative() ? 0 : 1)
        .ThenBy(mi => mi.Weight)
        .Select(mi => mi.Module)
        .ToList();

      return list;
    }

    public static void FixSequence(MBBindingList<LauncherModuleVM> list) {
      var vmDict = list
        .ToDictionary(vm => vm.Info.Id);

      var copy = list.ToList();

      var nativeIndex = copy.FindIndex(m => m.Info.IsNative());
      var native = copy[nativeIndex];
      if (nativeIndex != 0) {
        copy.RemoveAt(nativeIndex);
        copy.Insert(0, native);
      }

      var sorted = copy
        .StableOrderTopologicallyBy(mod => mod.Info.DependedModuleIds.Select(id => vmDict[id]))
        .ToList();

      // don't want to fire a bunch of gui update events for clear/inserts, sort is just one
      IdentitySort(list, sorted);
    }

    private static void IdentitySort(MBBindingList<LauncherModuleVM> list, IReadOnlyList<LauncherModuleVM> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.IndexOf(a)
          .CompareTo(sorted.IndexOf(b))));

    private static void IdentitySort(MBBindingList<LauncherModuleVM> list, IReadOnlyList<ModuleInfo> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.FindIndex(x => x == a.Info)
          .CompareTo(sorted.FindIndex(x => x == b.Info))));

    public static IReadOnlyList<ModuleInfo> ModuleList { get; private set; }

    private static void UnblockModule(ModuleInfo moduleInfo) {
      var binDir = new Uri(Path.Combine(
        Environment.CurrentDirectory,
        Path.GetDirectoryName(ModuleInfo.GetPath(moduleInfo.Alias)),
        "bin", Common.ConfigName
      )).LocalPath;

      if (!Directory.Exists(binDir))
        return;

      foreach (var file in Directory.EnumerateFiles(binDir)) {
        var ext = Path.GetExtension(file);
        if (ext == ".dll")
          if (SecurityHelpers.UnblockFile(file))
            Console.WriteLine("Unblocked: " + file);
      }
    }

    private static bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo) {
      if (isMultiplayer && moduleInfo.IsMultiplayerModule)
        return true;

      return !isMultiplayer && moduleInfo.IsSingleplayerModule;
    }

    public static bool ChangeLoadingOrderOfPrefix(LauncherModsVM __instance, LauncherModuleVM targetModule, int insertIndex, string tag) {
      var index = __instance.Modules.IndexOf(targetModule);
      if (insertIndex == index)
        return false;

      if (insertIndex > index)
        insertIndex--;
      insertIndex = (int) MathF.Clamp(insertIndex, 0f, __instance.Modules.Count - 1);
      __instance.Modules.RemoveAt(index);
      __instance.Modules.Insert(insertIndex, targetModule);
      FixSequence(__instance.Modules);
      return false;
    }

    // NOTE: this doesn't actually get used
    private static void ChangeLoadingOrderOf(LauncherModsVM mods, LauncherModuleVM targetModule, int insertIndex, string tag)
      => ChangeLoadingOrderOfPrefix(mods, targetModule, insertIndex, tag);

    private static void ChangeIsSelectedOf(LauncherModsVM mods, UserData userData, bool isMultiplayer, LauncherModuleVM targetModule) {
      if (targetModule.IsSelected) {
        using (var enumerator = mods.Modules.GetEnumerator()) {
          while (enumerator.MoveNext()) {
            var launcherModuleVm = enumerator.Current;
            if (launcherModuleVm == null)
              continue;

            launcherModuleVm.IsSelected |= targetModule.Info.DependedModuleIds.Contains(launcherModuleVm.Info.Id);
          }

          return;
        }
      }

      foreach (var launcherModuleVm2 in mods.Modules) {
        launcherModuleVm2.IsSelected &= !launcherModuleVm2.Info.DependedModuleIds.Contains(targetModule.Info.Id);
      }
    }

  }

}