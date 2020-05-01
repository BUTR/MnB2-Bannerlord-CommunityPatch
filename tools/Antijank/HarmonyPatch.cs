using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace Antijank {

  static internal class HarmonyPatch {

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool HarmonyFileLogPatch(string str) {
      Console.WriteLine(new string(FileLog.indentChar, FileLog.indentLevel) + str);
      return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool HarmonyFileLogListPatch(List<string> strings) {
      foreach (var line in strings)
        Console.WriteLine(new string(FileLog.indentChar, FileLog.indentLevel) + line);
      return false;
    }

    public static void PatchHarmonyLogging() {
      Console.WriteLine("Attempting to direct Harmony logging output to console.");
      try {
        Context.Harmony.Patch(typeof(FileLog).GetMethod(nameof(FileLog.Log)),
          new HarmonyMethod(typeof(HarmonyPatch), nameof(HarmonyFileLogPatch)));
        Context.Harmony.Patch(typeof(FileLog).GetMethod(nameof(FileLog.LogBuffered), new[] {typeof(string)}),
          new HarmonyMethod(typeof(HarmonyPatch), nameof(HarmonyFileLogPatch)));
        Context.Harmony.Patch(typeof(FileLog).GetMethod(nameof(FileLog.LogBuffered), new[] {typeof(List<string>)}),
          new HarmonyMethod(typeof(HarmonyPatch), nameof(HarmonyFileLogListPatch)));
        Context.Harmony.Patch(typeof(FileLog).GetMethod(nameof(FileLog.FlushBuffer)),
          new HarmonyMethod(typeof(HarmonyPatch), nameof(DoNothing)));
        Context.Harmony.Patch(typeof(FileLog).GetMethod(nameof(FileLog.Reset)),
          new HarmonyMethod(typeof(HarmonyPatch), nameof(DoNothing)));
        FileLog.Log("Logging Harmony output to console success.");
        FileLog.FlushBuffer();
        if (Options.EnableHarmonyDebugLogging)
          Harmony.DEBUG = true;
      }
      catch (Exception e) {
        Console.WriteLine("Logging Harmony output to console failed.");
        Logging.Log(e);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool DoNothing()
      => false;

  }

}