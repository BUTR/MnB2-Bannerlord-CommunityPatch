using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;
using Medallion.Collections;
using Antijank.Interop;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.TwoDimension.Standalone;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;
using Module = TaleWorlds.MountAndBlade.Module;
using Path = System.IO.Path;

namespace Antijank {

  internal static class LoaderPatch {

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static int InitCount = 0;

    static LoaderPatch() {
      if (InitCount > 0) {
        if (Debugger.IsAttached)
          Debugger.Break();
        throw new InvalidOperationException("Multiple static initializer runs!");
      }

      ++InitCount;
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

      Context.Harmony.Patch(AccessTools.Method(typeof(Module), "CollectModuleAssemblyTypes"),
        finalizer: new HarmonyMethod(typeof(LoaderPatch), nameof(CollectModuleAssemblyTypesFinalizer)));

      Context.Harmony.Patch(AccessTools.Method(typeof(Module), "InitializeSubModules"),
        transpiler: new HarmonyMethod(typeof(LoaderPatch), nameof(InitializeSubModulesTranspiler)));
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

    private static UserDataManager _userDataManager;

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
      _userDataManager = userDataManager;

      var input = context.EventManager.InputContext;

      void KeyWatcher(float tick) {
        SynchronizationContext.Current.Post(_
          => context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5), null);

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
          if (!string.IsNullOrWhiteSpace(inputText)) {
            var ids = inputText.Split(new[] {
              ", ", ",",
              "; ", ";",
              "\r\n",
              "\r", "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            var idSet = new HashSet<string>(ids);

            var dict = Loader.ModuleList.ToDictionary(mi => mi.Id);
            var missingDeps = new HashSet<ModuleInfo>();
            var list = Loader.ModuleList
              .Where(mod => {
                var hasNoMissingDeps = mod.DependedModuleIds.All(id => dict.ContainsKey(id));
                if (!hasNoMissingDeps)
                  missingDeps.Add(mod);
                return hasNoMissingDeps;
              })
              .OrderTopologicallyBy(mod => mod
                .GetDependedModuleIds(Loader.ModuleList)
                .Select(id => dict[id]))
              .ThenBy(Loader.GetLoadGroup)
              .ThenBy(x => ids.Contains(x.Id) ? 0 : 1)
              .ToList();

            if (missingDeps.Count > 0) {
              var sb = new StringBuilder("Mods missing dependencies:\n");
              foreach (var mod in missingDeps) {
                mod.IsSelected = false;
                list.Add(mod);
                sb.Append(mod.Name).Append(": ").AppendLine(string.Join(", ", mod.DependedModuleIds.Where(id => !dict.ContainsKey(id))));
              }

              MessageBox.Warning(sb.ToString());
            }

            Loader.ModuleList = list;
            var moduleVms = launcherVm.ModsData.Modules;
            Loader.IdentitySort(moduleVms, list);
            foreach (var mod in moduleVms) {
              var info = mod.Info;
              if (info.IsOfficial)
                continue;

              var id = info.Id;
              mod.IsSelected = idSet.Contains(id);
            }
          }

          _waitForKeysReset = true;
        }

        if (input.IsKeyDown(InputKey.O)) {
          IFileOpenDialog ofd = new FileOpenDialogClass();
          var savesDir = Path.GetDirectoryName(PathHelpers.GetSavesDir());
          var savesDirItem = ShellItemHelpers.Parse(savesDir);
          ofd.SetDefaultFolder(savesDirItem);
          ofd.SetOptions(
            FileDialogOptions.PathMustExist
            | FileDialogOptions.FileMustExist
            | FileDialogOptions.NoChangeDir
            | FileDialogOptions.ForceFileSystem
            | FileDialogOptions.HideMruPlaces
            | FileDialogOptions.HidePinnedPlaces
            | FileDialogOptions.NoDereferenceLinks
            | FileDialogOptions.DontAddToRecent
            | FileDialogOptions.NoValidate
            | FileDialogOptions.StrictFileTypes
          );
          ofd.SetFileTypes(1, new[] {
            new COMDLG_FILTERSPEC {
              pszName = "Save Games",
              pszSpec = "*.sav"
            }
          });
          ofd.SetTitle("Select Save Game");
          ofd.SetOkButtonLabel("Read Modules List");
          var cancelled = false;
          try {
            ofd.Show(default);
          }
          catch (COMException cex) when (cex.ErrorCode == unchecked((int) 0x800704C7)) {
            // operation cancelled by user; hit cancel button or closed window
            cancelled = true;
          }

          IShellItem picked = null;
          if (!cancelled)
            ofd.GetResult(out picked);

          if (picked == null)
            return;

          picked.GetDisplayName(ShellItemDisplayName.FileSysPath, out var path);
          var metaData = SaveManager.LoadMetaData(new FileDriver(path));
          var modsNamesList = metaData.GetModules();
          Console.WriteLine(string.Join(", ", modsNamesList));

          var mods = modsNamesList
            .Select(name => (Name: name, Id: Loader.ModuleList.FirstOrDefault(m => m.Name == name)?.Id))
            .ToList();

          var missingMods = mods
            .Where(m => m.Id == null).Select(m => m.Name)
            .ToList();

          var foundMods = mods
            .Where(m => m.Id != null)
            .Select(m => m.Id)
            .ToList();

          if (missingMods.Count > 0) {
            if (MessageBox.Warning("Missing modules from save:\n"
              + $"{string.Join("\n", missingMods)}\n\n"
              + $"Do you want to select the remaining {foundMods.Count} module(s)?\n"
              + "Any mods not listed will be deselected.",
              "Missing Modules From Save Game",
              MessageBoxType.YesNo,
              () => {
                MessageBox.Info("Complete list of modules in save:\n\n"
                  + $"{string.Join("\n", modsNamesList)}");
              }) != MessageBoxResult.Yes)
              return;
          }
          else {
            if (MessageBox.Warning("Module list from save:\n"
              + $"{string.Join("\n", modsNamesList)}\n\n"
              + $"Do you want to select the {foundMods.Count} module(s)?\n"
              + "Any mods not listed will be deselected.",
              "Missing Modules From Save Game",
              MessageBoxType.YesNo
            ) != MessageBoxResult.Yes)
              return;
          }

          //Loader.IdentitySort(list, foundMods);
          var dict = Loader.ModuleList.ToDictionary(mi => mi.Id);
          var missingDeps = new HashSet<ModuleInfo>();
          var list = Loader.ModuleList
            .Where(mod => {
              var hasNoMissingDeps = mod.DependedModuleIds.All(id => dict.ContainsKey(id));
              if (!hasNoMissingDeps)
                missingDeps.Add(mod);
              return hasNoMissingDeps;
            })
            .OrderTopologicallyBy(mod => mod
              .GetDependedModuleIds(Loader.ModuleList)
              .Select(id => dict[id]))
            .ThenBy(Loader.GetLoadGroup)
            .ThenBy(x => foundMods.Contains(x.Id) ? 0 : 1)
            .ToList();

          if (missingDeps.Count > 0) {
            var sb = new StringBuilder("Mods missing dependencies:\n");
            foreach (var mod in missingDeps) {
              mod.IsSelected = false;
              list.Add(mod);
              sb.Append(mod.Name).Append(": ").AppendLine(string.Join(", ", mod.DependedModuleIds.Where(id => !dict.ContainsKey(id))));
            }

            MessageBox.Warning(sb.ToString());
          }

          Loader.ModuleList = list;
          var idSet = new HashSet<string>(foundMods);
          ref var launcherVm = ref _launcherVmAccessor(__instance);
          var moduleVms = launcherVm.ModsData.Modules;
          Loader.IdentitySort(moduleVms, list);
          foreach (var mod in moduleVms) {
            var info = mod.Info;
            if (info.IsOfficial)
              continue;

            var id = info.Id;
            mod.IsSelected = idSet.Contains(id);
          }
        }
      }

      context.EventManager.AddLateUpdateAction(context.Root, KeyWatcher, 5);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
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

    [MethodImpl(MethodImplOptions.NoInlining)]
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
            .GetDependedModuleIds(Loader.ModuleList)
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CollectModuleAssemblyTypesFinalizer(ref Exception __exception, ref Dictionary<string, Type> __result, Assembly moduleAssembly) {
      var ex = __exception;
      if (ex != null) {
        var isMod = PathHelpers.IsModuleAssembly(moduleAssembly, out var mod);
        MessageBox.Warning("A problem occurred while scanning a module's types.\n"
          + $"Module: {(isMod ? mod.Name : "Not a mod.")}\n"
          + $"Assembly: {moduleAssembly.GetName().Name}",
          "Assembly Type Collection Failure",
          help: () => {
            MessageBox.Info(ex.ToString(), "Exception Details");
          });
      }

      __result ??= new Dictionary<string, Type>();
    }

    private static readonly AccessTools.FieldRef<Module, List<MBSubModuleBase>> ModuleSubModulesField
      = AccessTools.FieldRefAccess<Module, List<MBSubModuleBase>>("_submodules");

    private static readonly MethodInfo MbSubModuleBaseOnSubModuleLoadMethod
      = AccessTools.Method(typeof(MBSubModuleBase), "OnSubModuleLoad");

    private static readonly MethodInfo OnSubModuleLoadInterceptorMethod = AccessTools.Method(typeof(LoaderPatch), nameof(OnSubModuleLoadInterceptor));

    private static readonly MethodInfo SubModuleCtorInterceptorMethod = AccessTools.Method(typeof(LoaderPatch), nameof(SubModuleCtorInterceptor));

    private static readonly MethodInfo ConstructorInfoInvokeMethod = AccessTools.Method(typeof(ConstructorInfo), nameof(ConstructorInfo.Invoke), new[] {typeof(object[])});

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static object SubModuleCtorInterceptor(ConstructorInfo ctor, object[] args) {
      try {
        return ctor.Invoke(args);
      }
      catch (Exception ex) {
        var type = ctor.DeclaringType;
        var asm = type!.Assembly;
        var isMod = PathHelpers.IsModuleAssembly(asm, out var mod);
        var choice = MessageBox.Error("A problem occurred while constructing a submodule.\n"
          + $"Submodule: {type.AssemblyQualifiedName}\n"
          + $"Module: {(isMod ? mod.Name : "Not a mod")}\n"
          + "Would you like to proceed without it?",
          "Submodule Load Failure",
          MessageBoxType.YesNo,
          () => {
            MessageBox.Info($"{ex}", "Exception Details");
          });

        if (choice == MessageBoxResult.Yes) // bye bye
          return new RemovedSubModule();

        throw;
      }
    }

    private class RemovedSubModule : MBSubModuleBase {

      protected override void OnSubModuleLoad()
        => ModuleSubModulesField(Module.CurrentModule).Remove(this);

    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void OnSubModuleLoadInterceptor(MBSubModuleBase subModule) {
      void InvokeOnSubModuleLoad() {
        var retry = false;
        try {
          MbSubModuleBaseOnSubModuleLoadMethod.Invoke(subModule, new object[0]);
        }
        catch (Exception ex) {
          var type = subModule.GetType();
          var asm = type.Assembly;
          var isMod = PathHelpers.IsModuleAssembly(asm, out var mod);
          var choice = MessageBox.Error("A problem occurred while loading a submodule.\n"
            + $"Submodule: {type.AssemblyQualifiedName}\n"
            + $"Module: {(isMod ? mod.Name : "Not a mod")}\n"
            + "How would you like to proceed?",
            "Submodule Load Failure",
            MessageBoxType.CancelTryAgainContinue,
            () => {
              MessageBox.Info("Cancel will unload the submodule.\n"
                + "Try Again will reload the submodule.\n"
                + "Continue will ignore the exception.\n\n"
                + $"{ex}", "Exception Details");
            });
          switch (choice) {
            case MessageBoxResult.Cancel: {
              // bye bye
              ModuleSubModulesField().Remove(subModule);
              break;
            }
            case MessageBoxResult.TryAgain: {
              // why not
              retry = true;
              break;
            }
            case MessageBoxResult.Continue: {
              // well ok
              break;
            }
          }
        }

        if (retry)
          SynchronizationContext.Current.Post(_ => InvokeOnSubModuleLoad(), null);
      }

      InvokeOnSubModuleLoad();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IEnumerable<CodeInstruction> InitializeSubModulesTranspiler(IEnumerable<CodeInstruction> instr) {
      foreach (var il in instr) {
        var method = il.operand as MethodInfo;

        if (method == ConstructorInfoInvokeMethod) {
          il.operand = SubModuleCtorInterceptorMethod;
          il.opcode = OpCodes.Call;
          Console.WriteLine("Hooked SubModule constructor invocation.");
        }
        else if (method == MbSubModuleBaseOnSubModuleLoadMethod) {
          il.operand = OnSubModuleLoadInterceptorMethod;
          il.opcode = OpCodes.Call;
          Console.WriteLine("Hooked OnSubModuleLoad event.");
        }

        yield return il;
      }
    }

  }

}