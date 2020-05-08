using System;
using System.Collections.Generic;
using Antijank.Debugging;
using HarmonyLib;
using TaleWorlds.Library;

namespace Antijank {

  public class ModuleFileScanningPatch {

    static ModuleFileScanningPatch()
      => Context.Harmony.Patch(AccessTools.Method(typeof(ModuleInfo), nameof(ModuleInfo.Load)),
        finalizer: new HarmonyMethod(typeof(ModuleFileScanningPatch), nameof(ModuleInfoLoadFinalizer)));

    public static void Init() {
      // run static initializer
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