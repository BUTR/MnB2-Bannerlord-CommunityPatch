using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;
using Medallion.Collections;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.TwoDimension.Standalone;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;
using Path = System.IO.Path;

namespace Antijank {

  internal static class LoaderPatch {

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    static LoaderPatch() {
      Context.Harmony.Patch(
        typeof(LauncherModsVM).GetMethod("LoadSubModules", Declared),
        new HarmonyMethod(typeof(LoaderPatch), nameof(LoadSubModulesPrefix)));

      Context.Harmony.Patch(
        typeof(LauncherModsVM).GetMethod("ChangeLoadingOrderOf", Declared),
        new HarmonyMethod(typeof(LoaderPatch), nameof(ChangeLoadingOrderOfPrefix)));

      Context.Harmony.Patch(
        typeof(LauncherUI).GetConstructors(Declared).First(),
        postfix: new HarmonyMethod(typeof(LoaderPatch), nameof(LauncherUiCtorPostfix)));

      Context.Harmony.Patch(
        typeof(LauncherUI).GetMethod("Initialize", Declared),
        postfix: new HarmonyMethod(typeof(LoaderPatch), nameof(LauncherUiInitializePostfix)));

      Context.Harmony.Patch(
        typeof(GraphicsForm).GetMethod("UpdateInput", Declared),
        new HarmonyMethod(typeof(LoaderPatch), nameof(GraphicsFormUpdateInputPatch)));

      Context.Harmony.Patch(
        typeof(GraphicsForm).GetMethod("MessageHandler", Declared),
        new HarmonyMethod(typeof(LoaderPatch), nameof(GraphicsFormMessageHandlerPatch)));
    }

    private static readonly byte[] VirtualKeyCodeToInputKeyMap = new byte[256];

    public static void Init() {
      // static initializer

      for (var i = 1; i <= 255; ++i) {
        var name = Enum.GetName(typeof(InputKey), (InputKey) i);
        if (Enum.TryParse(name, out VirtualKeyCode vk) && (int) vk <= 255)
          VirtualKeyCodeToInputKeyMap[(byte) vk] = (byte) i;
      }
    }

    private static AccessTools.FieldRef<LauncherUI, LauncherVM> _launcherVmAccessor;

    private static AccessTools.FieldRef<LauncherUI, GauntletMovie> _gauntletMovieAccessor;

    private static GauntletMovie _movie;

    private static LauncherVM _launcherVm;

    [DllImport("user32")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    private static bool GraphicsFormMessageHandlerPatch(WindowMessage message, ref long wParam, long lParam) {
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
        : 0; // unused slot on TW's key map

      return true;
    }

    private static unsafe bool GraphicsFormUpdateInputPatch(
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

    private static bool _waitForKeysReset;

    private static void LauncherUiInitializePostfix(LauncherUI __instance) {
      _launcherVm = _launcherVmAccessor(__instance);
      _movie = _gauntletMovieAccessor(__instance);
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static void LauncherUiCtorPostfix(LauncherUI __instance, UserDataManager userDataManager, UIContext context, Action onClose, Action onMinimize) {
      //_launcherUi = __instance;
      //_uiContext = context;
      _launcherVmAccessor = AccessTools.FieldRefAccess<LauncherUI, LauncherVM>("_viewModel");
      _gauntletMovieAccessor = AccessTools.FieldRefAccess<LauncherUI, GauntletMovie>("_movie");

      var input = context.EventManager.InputContext;

      void KeyWatcher(float tick) {
        SynchronizationContext.Current.Post(_ => {
          context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5);
        }, null);

        var ctrl = input.IsControlDown();

        if (_waitForKeysReset) {
          if (!ctrl)
            _waitForKeysReset = false;
          return;
        }

        if (!ctrl)
          return;

        if (input.IsKeyDown(InputKey.S)) {
          _waitForKeysReset = true;
          Console.WriteLine("Sorting modules list.");

          var list = Loader.DefaultSort();

          var launcherModuleVms = _launcherVm.ModsData.Modules;
          Loader.IdentitySort(launcherModuleVms, list);
          _movie.RefreshDataSource(_launcherVm);
          return;
        }

        if (input.IsKeyDown(InputKey.C)) {
          Console.WriteLine("Copying modules list to clipboard.");
          var modsList = string.Join("\n", Loader.ModuleList
            .Where(m => m.IsSelected)
            .Select(m => m.Id));
          TextCopy.Clipboard.SetText(modsList);
          _waitForKeysReset = true;
          return;
        }

        if (input.IsKeyDown(InputKey.V)) {
          Console.WriteLine("Pasting modules list from clipboard.");
          ref var launcherVm = ref _launcherVmAccessor(__instance);
          var inputText = TextCopy.Clipboard.GetText() ?? "";
          var ids = inputText.Split(new[] {
            ", ", ",",
            "; ", ";",
            "\r\n",
            "\r", "\n"
          }, StringSplitOptions.RemoveEmptyEntries);
          var idSet = new HashSet<string>(ids);
          Loader.IdentitySort(launcherVm.ModsData.Modules, ids);
          foreach (var mod in launcherVm.ModsData.Modules) {
            var info = mod.Info;
            if (info.IsOfficial)
              continue;

            var id = info.Id;
            mod.IsSelected = idSet.Contains(id);
          }

          Loader.FixSequence(launcherVm.ModsData.Modules);
          _waitForKeysReset = true;
        }
      }

      context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5);
    }

    private static bool LoadSubModulesPrefix(LauncherModsVM __instance, UserData ____userData, bool isMultiplayer) {
      var list = Loader.GetOrderedModuleList(____userData, isMultiplayer);

      Loader.ModuleList = new MBReadOnlyList<ModuleInfo>(list);

      foreach (var module in Loader.ModuleList) {
        Loader.UnblockModule(module);
        var launcherModuleVm = new LauncherModuleVM(module,
          // NOTE: this doesn't actually get used
          (targetModule, insertIndex, tag) => ChangeLoadingOrderOf(__instance, targetModule, insertIndex, tag),
          targetModule => ChangeIsSelectedOf(__instance, ____userData, isMultiplayer, targetModule));
        __instance.Modules.Add(launcherModuleVm);
      }

      return false;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static bool ChangeLoadingOrderOfPrefix(LauncherModsVM __instance, LauncherModuleVM targetModule, int insertIndex, string tag) {
      var index = __instance.Modules.IndexOf(targetModule);
      if (insertIndex == index)
        return false;

      if (insertIndex > index)
        insertIndex--;
      insertIndex = (int) MathF.Clamp(insertIndex, 0f, __instance.Modules.Count - 1);
      __instance.Modules.RemoveAt(index);
      __instance.Modules.Insert(insertIndex, targetModule);
      Loader.FixSequence(__instance.Modules);
      return false;
    }

    // NOTE: this doesn't actually get used
    private static void ChangeLoadingOrderOf(LauncherModsVM mods, LauncherModuleVM targetModule, int insertIndex, string tag)
      => ChangeLoadingOrderOfPrefix(mods, targetModule, insertIndex, tag);

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static void ChangeIsSelectedOf(LauncherModsVM mods, UserData userData, bool isMultiplayer, LauncherModuleVM targetModule) {
      if (targetModule.IsSelected)
        foreach (var module in mods.Modules) {
          if (module == null)
            continue;

          module.IsSelected |= targetModule.Info
            .GetDependedModuleIdsWithOptional(Loader.ModuleList)
            .Contains(module.Info.Id);
        }
      else
        foreach (var launcherModuleVm2 in mods.Modules) {
          launcherModuleVm2.IsSelected &= !launcherModuleVm2.Info
            .DependedModuleIds
            .Contains(targetModule.Info.Id);
        }

      Loader.FixSequence(mods.Modules);

      _movie.RefreshDataSource(_launcherVm);
    }

  }

}