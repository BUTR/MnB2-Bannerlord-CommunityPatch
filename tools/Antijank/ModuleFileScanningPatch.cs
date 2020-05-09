using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Antijank.Debugging;
using HarmonyLib;
using TaleWorlds.Library;
using Module = TaleWorlds.MountAndBlade.Module;

namespace Antijank {

  public class ModuleFileScanningPatch {

    static ModuleFileScanningPatch() {
      Context.Harmony.Patch(AccessTools.Method(typeof(ModuleInfo), nameof(ModuleInfo.Load)),
        finalizer: new HarmonyMethod(typeof(ModuleFileScanningPatch), nameof(ModuleInfoLoadFinalizer)));

      Context.Harmony.Patch(AccessTools.Method(typeof(Module), "CollectModuleAssemblyTypes"),
        finalizer: new HarmonyMethod(typeof(ModuleFileScanningPatch), nameof(CollectModuleAssemblyTypesFinalizer)));
    }

    public static void Init() {
      // run static initializer
    }

    public static void CollectModuleAssemblyTypesFinalizer(ref Exception __exception, Assembly moduleAssembly) {
      if (__exception == null)
        return;

      if (moduleAssembly.IsDynamic) {
        __exception = null;
        return;
      }

      var ex = CallStackHelpers.UnnestCommonExceptions(__exception);

      var isMod = PathHelpers.IsModuleAssembly(moduleAssembly, out var mod);

      var alias = isMod ? "<Unknown>" : mod.Alias;
      var excName = ex.GetType().Name;
      var excMsg = ex.Message;
      Console.WriteLine($"Module {alias} is unable to be scanned.");
      Console.WriteLine($"Assembly: {new Uri(moduleAssembly.CodeBase).LocalPath}");
      Console.WriteLine($"{excName}: {excMsg}");

      if (DontShowLoadFailure.TryGetValue(alias, out var prevChoice) && !prevChoice)
        return;

      DontShowLoadFailure[alias] = MessageBox.Error($"Module {alias} is unable to be scanned.\n"
        + $"Assembly: {new Uri(moduleAssembly.CodeBase).LocalPath}\n"
        + $"{excName}: {excMsg}\n\n"
        + "Continue showing loading errors for this module?", "Module Load Failure", MessageBoxType.YesNo, () => {
          MessageBox.Info(ex.ToString(), "Error Details");
        }) == MessageBoxResult.No;

      __exception = null;
      
    }
    
    private static readonly Dictionary<string, bool> DontShowLoadFailure
      = new Dictionary<string, bool>();

    public static void ModuleInfoLoadFinalizer(ref Exception __exception, string alias) {
      if (__exception == null)
        return;

      var excName = __exception.GetType().Name;
      var excMsg = __exception.Message;
      Console.WriteLine($"Module {alias} is unable to be scanned.");
      Console.WriteLine($"{excName}: {excMsg}");

      if (DontShowLoadFailure.TryGetValue(alias, out var prevChoice) && !prevChoice)
        return;

      var exc = __exception;

      DontShowLoadFailure[alias] = MessageBox.Error($"Module {alias} is unable to be scanned.\n"
        + $"{excName}: {excMsg}\n\n"
        + "Continue showing loading errors for this module?", "Module Load Failure", MessageBoxType.YesNo, () => {
          MessageBox.Info(exc.ToString(), "Error Details");
        }) == MessageBoxResult.No;

      __exception = null;
    }

  }

}