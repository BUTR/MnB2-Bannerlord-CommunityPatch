using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Antijank {

  public class ModuleFileScanningPatch {

    static ModuleFileScanningPatch() {
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(ModuleInfo), nameof(ModuleInfo.Load)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ModuleInfoLoadFinalizer)));
    }

    public static void Init() {
      // run static initializer
    }

    private static Dictionary<string, bool> _dontShowLoadFailure;

    public static void ModuleInfoLoadFinalizer(ref Exception __exception, string alias) {
      if (__exception == null)
        return;

      var excName = __exception.GetType().Name;
      var excMsg = __exception.Message;
      Console.WriteLine($"Module {alias} is unable to be scanned.");
      Console.WriteLine($"{excName}: {excMsg}");

      if (_dontShowLoadFailure.TryGetValue(alias, out var prevChoice) && !prevChoice)
        return;

      var exc = __exception;

      _dontShowLoadFailure[alias] = MessageBox.Error($"Module {alias} is unable to be scanned.\n"
        + $"{excName}: {excMsg}\n\n"
        + "Continue showing loading errors for this module?", "Module Load Failure", MessageBoxType.YesNo, () => {
          MessageBox.Info(exc.ToString(), "Error Details");
        }) == MessageBoxResult.No;

      __exception = null;
    }

  }

}