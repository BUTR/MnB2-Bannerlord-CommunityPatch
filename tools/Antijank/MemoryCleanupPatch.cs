using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace Antijank {

  public class MemoryCleanupPatch {

    private static int InitCount = 0;

    static MemoryCleanupPatch() {
      if (InitCount > 0) {
        if (Debugger.IsAttached)
          Debugger.Break();
        throw new InvalidOperationException("Multiple static initializer runs!");
      }

      ++InitCount;

      Context.Harmony.Patch(AccessTools.Method(typeof(Common), nameof(Common.MemoryCleanup)),
        new HarmonyMethod(typeof(MemoryCleanupPatch), nameof(MemoryCleanupReplacement)));
      Context.Harmony.Patch(AccessTools.Method(typeof(Managed), "GarbageCollect"),
        new HarmonyMethod(typeof(MemoryCleanupPatch), nameof(MemoryCleanupReplacement)));

      Context.Harmony.Patch(AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.LoadCallbackInitializator, TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral"), "InitializeObjects"),
        transpiler: new HarmonyMethod(typeof(MemoryCleanupPatch), nameof(GcCallsTranspiler)));

      Context.Harmony.Patch(AccessTools.Method(typeof(LoadContext), nameof(LoadContext.Load)),
        transpiler: new HarmonyMethod(typeof(MemoryCleanupPatch), nameof(GcCallsTranspiler)));

      Context.Harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.LoadSaveGame)),
        transpiler: new HarmonyMethod(typeof(MemoryCleanupPatch), nameof(GcCallsTranspiler)));
    }

    private static readonly MethodInfo GcCollectMethodInfo = AccessTools.Method(typeof(GC), nameof(GC.Collect), new Type[0]);

    private static readonly MethodInfo GcWaitForPendingFinalizersMethodInfo = AccessTools.Method(typeof(GC), nameof(GC.WaitForPendingFinalizers), new Type[0]);

    private static readonly MethodInfo MemoryCleanupMethodInfo = AccessTools.Method(typeof(MemoryCleanupPatch), nameof(MemoryCleanup), new Type[0]);

    public static void Init() {
      // run static initializer
    }

    private static IEnumerable<CodeInstruction> GcCallsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {
      Console.WriteLine($"Replacing explicit calls to GC via transpiling in {original.FullDescription()}");
      var changes = 0;
      foreach (var instr in instructions) {
        if (instr.Calls(GcCollectMethodInfo)) {
          instr.operand = MemoryCleanupMethodInfo;
          yield return instr;

          Console.WriteLine($"Replacing call to GC.Collect with call to MemoryCleanup.");
          ++changes;
          continue;
        }

        if (instr.Calls(GcWaitForPendingFinalizersMethodInfo)) {
          instr.operand = null;
          instr.opcode = OpCodes.Nop;
          yield return instr;

          Console.WriteLine($"Replacing call to GC.WaitForPendingFinalizers with a no-op.");
          ++changes;
          continue;
        }

        yield return instr;
      }

      Console.WriteLine($"Changed {changes} opcodes while transpiling {original.FullDescription()}");
    }

    public static Stopwatch LastMemoryCleanupTimer { get; private set; } = Stopwatch.StartNew();

    [MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.NoInlining)]
    public static bool MemoryCleanupReplacement() {
      MemoryCleanup();
      return false;
    }

    [Conditional("TRACE")]
    private static void Log(string msg)
      => Trace.WriteLine(msg);

    public static void MemoryCleanup() {
      // note: other threads
      if (LastMemoryCleanupTimer.ElapsedMilliseconds <= 5000) {
        Log("Skipping forced memory clean up.");
        return;
      }

      LastMemoryCleanupTimer.Restart();

      Log("Beginning forced memory clean up.");
      var sw = Stopwatch.StartNew();
#if TRACE
      var mem = GC.GetTotalMemory(false);
#endif
      GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
      GC.Collect(2, GCCollectionMode.Forced);
      WaitForPendingFinalizers(125);
#if TRACE
      Log($"Recovered {(mem - GC.GetTotalMemory(false)) / 1048576.0f:F2}mB");
#endif
      Log($"Incurred {sw.ElapsedMilliseconds}ms delay due to forced memory clean up.");
    }

    private static void WaitForPendingFinalizers(int timeout) {
      Log("Beginning forced wait finalizer queue completion.");
      var sw = Stopwatch.StartNew();
      using var mre = new ManualResetEventSlim(false, 0);

      var interrupted = false;
      var sleeper = Thread.CurrentThread;
      var interrupter = new Thread((o) => {
        var e = (ManualResetEventSlim) o;
        try {
          e.Wait(timeout);
          if (!e.IsSet)
            sleeper.Interrupt();
        }
        catch (ObjectDisposedException) {
          // ok
        }
      });
      interrupter.Start(mre);

      try {
        GC.WaitForPendingFinalizers();
      }
      catch (ThreadInterruptedException) {
        // ok
        interrupted = true;
      }

      mre.Set();
      interrupter.Join(timeout);
      Log(interrupted
        ? $"Waited {sw.ElapsedMilliseconds}ms for partial finalizer queue completion."
        : $"Waited {sw.ElapsedMilliseconds}ms for finalizer queue completion.");
    }

  }

}