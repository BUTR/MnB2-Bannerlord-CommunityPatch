using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using HardwareProviders.CPU;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using static CommunityPatch.CommunityPatchSubModule;
using Harmony = HarmonyLib.Harmony;
using Module = TaleWorlds.MountAndBlade.Module;
using Path = TaleWorlds.Engine.Path;

namespace CommunityPatch {

  public static class Diagnostics {

    private static readonly byte[] RandomIntBuf = new byte[4];

    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

    private static string _systemReportCache;

    private static Thread _queuedReportThread;

    private static DateTimeOffset _lastQueuedReport = DateTimeOffset.UtcNow;

    private static readonly TimeSpan QueuedReportThrottleTime = TimeSpan.FromMinutes(1);

    private static readonly TimeSpan QueuedReportWaitInterval = TimeSpan.FromSeconds(5);

    private static readonly TimeSpan MaxQueuedReportWaitTime = TimeSpan.FromMinutes(3);

    public static void QueueGenerateReport() {
      _lastQueuedReport = DateTimeOffset.UtcNow;

      if (_queuedReportThread != null)
        return;

      _queuedReportThread = new Thread(() => {
        var started = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow - _lastQueuedReport < QueuedReportThrottleTime) {
          Thread.Sleep(QueuedReportWaitInterval);
          if (DateTimeOffset.UtcNow - started > MaxQueuedReportWaitTime)
            break;
        }

        SynchronizationContext.Current.Post(_ => {
          GenerateReport();
          _queuedReportThread = null;
        }, null);
      }) {Name = "Queued Diagnostic Report", IsBackground = true};

      _queuedReportThread.Start();
    }

    public static void GenerateReport() {
      var sb = new StringBuilder();

      try {
        sb.AppendLine("Recorded Unhandled Exceptions:");
        var i = 0;
        foreach (var exc in RecordedUnhandledExceptions) {
          var excStr = exc.ToString();
          sb.Append("  ").Append(++i).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
          var iex = exc;
          var j = 0;
          while (iex.InnerException != null) {
            iex = iex.InnerException;
            sb.Append("    ").Append(i).Append(".").Append(++j).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
          }
        }

        if (i == 0)
          sb.AppendLine("  None.");
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        sb.AppendLine("Recorded First Chance Exceptions:");
        var i = 0;
        foreach (var exc in RecordedFirstChanceExceptions) {
          var excStr = exc.ToString();
          sb.Append("  ").Append(++i).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
          var iex = exc;
          var j = 0;
          while (iex.InnerException != null) {
            iex = iex.InnerException;
            sb.Append("    ").Append(i).Append(".").Append(++j).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
          }
        }

        if (RecordFirstChanceExceptions) {
          if (i == 0)
            sb.AppendLine("  None recorded.");
        }
        else
          sb.AppendLine("  Recording disabled.");
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        sb.AppendLine("Modules Information:");
        var i = 0;
        foreach (var mi in ModuleInfo.GetModules()) {
          sb.Append("  ").Append(++i).Append(". ").Append(mi.Id).Append(" ").Append(mi.Version.ToString());
          if (mi.IsSelected)
            sb.Append(" *Selected*");
          sb.AppendLine();
          sb.Append("    ").Append(mi.Name);
          if (!string.IsNullOrWhiteSpace(mi.Alias))
            sb.Append(" (").Append(mi.Alias).Append(")");
          if (mi.IsOfficial)
            sb.Append(" *Official*");
          sb.AppendLine();
          if (mi.DependedModuleIds.Count <= 0)
            continue;

          sb.Append("  ").AppendLine("Dependencies:");
          var j = 0;
          foreach (var dep in mi.DependedModuleIds)
            sb.Append("    ").Append(++j).Append(". ").AppendLine(dep);
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        sb.AppendLine("Harmony Patch Information:");
        var i = 0;
        foreach (var patchedMethod in Harmony.GetAllPatchedMethods()) {
          var patchInfo = Harmony.GetPatchInfo(patchedMethod);
          sb.Append("  ").Append(++i).Append(". ").AppendLine(patchedMethod.FullDescription());

          if (patchInfo.Prefixes.Count > 0) {
            var j = 0;
            sb.Append("    ").AppendLine("Prefixes:");
            foreach (var patch in patchInfo.Prefixes) {
              sb.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
              sb.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
            }
          }

          if (patchInfo.Postfixes.Count > 0) {
            var j = 0;
            sb.Append("    ").AppendLine("Postfixes:");
            foreach (var patch in patchInfo.Postfixes) {
              sb.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
              sb.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
            }
          }

          if (patchInfo.Finalizers.Count > 0) {
            var j = 0;
            sb.Append("    ").AppendLine("Finalizers:");
            foreach (var patch in patchInfo.Finalizers) {
              sb.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
              sb.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
            }
          }

          if (patchInfo.Transpilers.Count > 0) {
            var j = 0;
            sb.Append("    ").AppendLine("Transpilers:");
            foreach (var patch in patchInfo.Transpilers) {
              sb.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
              sb.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
            }
          }
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        sb.AppendLine("Community Patch Information:");
        var i = 0;
        foreach (var patch in CommunityPatchSubModule.Patches) {
          var type = patch.GetType();
          sb.Append("  ").Append(++i).Append(". ").Append(type.Name);
          if (ActivePatches.ContainsKey(type))
            sb.Append(" *Active*");
          var applicability = patch.IsApplicable(Game.Current);
          if (applicability ?? false)
            sb.Append(" *Applicable*");
          if (applicability == null)
            sb.Append(" *Maybe Applicable*");
          if (patch.Applied)
            sb.Append(" *Applied*");
          sb.AppendLine();
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        sb.AppendLine("Loaded SubModules:");
        var i = 0;
        foreach (var sm in Module.CurrentModule.SubModules) {
          try {
            var type = sm.GetType();
            var asm = type.Assembly;
            sb.Append("  ").Append(++i).Append(". ").AppendLine(type.AssemblyQualifiedName);
            foreach (var version in asm.GetCustomAttributes<AssemblyInformationalVersionAttribute>())
              sb.Append("    v").AppendLine(version.InformationalVersion);
          }
          catch (Exception ex) {
            sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
          }
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      AppendSystemReport(sb);

      try {
        var reportStr = sb.ToString();

        ShowMessage("Saving to \"My Documents\\Mount and Blade II Bannerlord\\diagnostic-report.txt\"");

        try {
          var docsMnb2 = new Uri(System.IO.Path.Combine(PathHelpers.GetConfigsDir(), "..")).LocalPath;
          var now = DateTime.UtcNow;
          Random.GetNonZeroBytes(RandomIntBuf);
          var randomInt = Unsafe.ReadUnaligned<uint>(ref RandomIntBuf[0]);
          File.WriteAllText(System.IO.Path.Combine(docsMnb2, $"diagnostic-report.{now.Year:0000}{now.Month:00}{now.Day:00}{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond}.{randomInt:X8}.txt"), reportStr);
        }
        catch (Exception ex2) {
          ShowMessage($"Failed to save diagnostic report!\n{ex2.GetType().Name}: {ex2.Message}");
        }

        try {
          Input.SetClipboardText(reportStr);
          ShowMessage("Diagnostics also copied to system clipboard.");
        }
        catch (Exception ex) {
          ShowMessage($"Writing to system clipboard failed!\n{ex.GetType().Name}: {ex.Message}");
        }
      }
      catch (Exception ex) {
        ShowMessage($"Failed to generate string from diagnostic report string buffer!\n{ex.GetType().Name}: {ex.Message}");
      }
    }

    private static void AppendSystemReport(StringBuilder sb) {
      if (_systemReportCache != null) {
        sb.Append(_systemReportCache);
        return;
      }

      var start = sb.Length;
      try {
        sb.AppendLine("System Info:");
        sb.Append("  ").AppendLine(Utilities.GetPCInfo().Replace("\n", "\n  "));
        sb.Append($"  GPU Memory: {Utilities.GetGPUMemoryMB()}MB").AppendLine();
        sb.Append($"  GC Allocated: {GC.GetTotalMemory(false)}B").AppendLine();
        sb.Append($"  Engine Memory Used: {Utilities.GetCurrentCpuMemoryUsageMB()}MB").AppendLine();
        sb.Append("  GC Latency Mode: ").Append(GCSettings.IsServerGC ? "Server " : "Client ").AppendLine(GCSettings.LatencyMode.ToString());
        sb.AppendFormat($"  GC LOH Compact Mode: {GCSettings.LargeObjectHeapCompactionMode}").AppendLine();
        sb.AppendFormat($"  Operating System: {RuntimeInformation.OSDescription}").AppendLine();
        sb.AppendFormat($"  Framework Compatibility: {RuntimeInformation.FrameworkDescription}").AppendLine();
        sb.AppendFormat($"  Hardware Accelerated Vector: {(Vector.IsHardwareAccelerated ? "Yes" : "No")}").AppendLine();
        sb.AppendFormat($"  Vector Size: {Vector<byte>.Count * 8} bits").AppendLine();
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      try {
        var cpus = Cpu.Discover();

        sb.AppendLine("  CPU Info:");
        for (var i = 0; i < cpus.Length; ++i) {
          var cpu = cpus[i];
          cpu.Update();
          var coreCount = cpu.CoreCount;

          for (var c = 0; c < coreCount; ++c) {
            cpu.ActivateSensor(cpu.CoreClocks[c]);
            cpu.ActivateSensor(cpu.CoreTemperatures[c]);
          }

          Thread.Sleep(100);

          sb.Append("    ").Append(i + 1).Append(". ").Append(cpu.Name).Append(" with ").Append(coreCount).AppendLine(" cores:");
          for (var c = 0; c < coreCount; ++c) {
            sb.Append("      ").Append(c + 1).Append(". ")
              .Append(cpu.CoreClocks[c].Max?.ToString(CultureInfo.InvariantCulture) ?? "(?)").Append(" MHz ")
              .Append(cpu.CoreTemperatures[c].Max?.ToString(CultureInfo.InvariantCulture) ?? "(?)").Append(" °C");
            sb.AppendLine();
          }

          for (var c = 0; c < coreCount; ++c) {
            cpu.DeactivateSensor(cpu.CoreClocks[c]);
            cpu.DeactivateSensor(cpu.CoreTemperatures[c]);
          }
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        _systemReportCache = sb.ToString(start, sb.Length - start);
      }
      catch {
        // out of memory?
      }
    }

  }

}