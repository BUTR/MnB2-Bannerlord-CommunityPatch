using System;
using System.Globalization;
using System.IO;
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
using Microsoft.IO;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using static CommunityPatch.CommunityPatchSubModule;
using Harmony = HarmonyLib.Harmony;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  public static class Diagnostics {

    public static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager(81920, 1, 81920);

    private static readonly byte[] RandomIntBuf = new byte[4];

    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

    private static byte[] _systemReportCache;

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

    private static StreamWriter Append(this StreamWriter sw, string str) {
      sw.Write(str);
      return sw;
    }

    private static StreamWriter Append(this StreamWriter sw, int str) {
      sw.Write(str);
      return sw;
    }

    private static StreamWriter AppendLine(this StreamWriter sw) {
      sw.WriteLine();
      return sw;
    }

    private static StreamWriter AppendLine(this StreamWriter sw, string str) {
      sw.WriteLine(str);
      return sw;
    }

    private static StreamWriter AppendFormat(this StreamWriter sw, FormattableString str) {
      sw.Write(str);
      return sw;
    }

    public static void GenerateReport() {
      using var ms = RecyclableMemoryStreamManager.GetStream(nameof(Diagnostics));
      using (var sw = new StreamWriter(ms, Encoding.UTF8, 81920, true) {AutoFlush = true}) {
        try {
          sw.AppendLine("Recorded Unhandled Exceptions:");
          var i = 0;
          foreach (var exc in RecordedUnhandledExceptions) {
            var excStr = exc.ToString();
            sw.Append("  ").Append(++i).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
            var iex = exc;
            var j = 0;
            while (iex.InnerException != null) {
              iex = iex.InnerException;
              sw.Append("    ").Append(i).Append(".").Append(++j).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
            }
          }

          if (i == 0)
            sw.AppendLine("  None.");
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        try {
          sw.AppendLine("Recorded First Chance Exceptions:");
          var i = 0;
          foreach (var exc in RecordedFirstChanceExceptions) {
            var excStr = exc.ToString();
            sw.Append("  ").Append(++i).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
            var iex = exc;
            var j = 0;
            while (iex.InnerException != null) {
              iex = iex.InnerException;
              sw.Append("    ").Append(i).Append(".").Append(++j).Append(". ").AppendLine(excStr.Replace("\n", "\n    "));
            }
          }

          if (RecordFirstChanceExceptions) {
            if (i == 0)
              sw.AppendLine("  None recorded.");
          }
          else
            sw.AppendLine("  Recording disabled.");
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        try {
          sw.AppendLine("Modules Information:");
          var i = 0;
          foreach (var mi in ModuleInfo.GetModules()) {
            sw.Append("  ").Append(++i).Append(". ").Append(mi.Id).Append(" ").Append(mi.Version.ToString());
            if (mi.IsSelected)
              sw.Append(" *Selected*");
            sw.AppendLine();
            sw.Append("    ").Append(mi.Name);
            if (!string.IsNullOrWhiteSpace(mi.Alias))
              sw.Append(" (").Append(mi.Alias).Append(")");
            if (mi.IsOfficial)
              sw.Append(" *Official*");
            sw.AppendLine();
            if (mi.DependedModuleIds.Count <= 0)
              continue;

            sw.Append("  ").AppendLine("Dependencies:");
            var j = 0;
            foreach (var dep in mi.DependedModuleIds)
              sw.Append("    ").Append(++j).Append(". ").AppendLine(dep);
          }
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        try {
          sw.AppendLine("Harmony Patch Information:");
          var i = 0;
          foreach (var patchedMethod in Harmony.GetAllPatchedMethods()) {
            var patchInfo = Harmony.GetPatchInfo(patchedMethod);
            sw.Append("  ").Append(++i).Append(". ").AppendLine(patchedMethod.FullDescription());

            if (patchInfo.Prefixes.Count > 0) {
              var j = 0;
              sw.Append("    ").AppendLine("Prefixes:");
              foreach (var patch in patchInfo.Prefixes) {
                sw.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
                sw.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
              }
            }

            if (patchInfo.Postfixes.Count > 0) {
              var j = 0;
              sw.Append("    ").AppendLine("Postfixes:");
              foreach (var patch in patchInfo.Postfixes) {
                sw.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
                sw.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
              }
            }

            if (patchInfo.Finalizers.Count > 0) {
              var j = 0;
              sw.Append("    ").AppendLine("Finalizers:");
              foreach (var patch in patchInfo.Finalizers) {
                sw.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
                sw.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
              }
            }

            if (patchInfo.Transpilers.Count > 0) {
              var j = 0;
              sw.Append("    ").AppendLine("Transpilers:");
              foreach (var patch in patchInfo.Transpilers) {
                sw.Append("      ").Append(++j).Append(". ").AppendLine(patch.PatchMethod.FullDescription());
                sw.Append("        ").AppendLine($"Owner: {patch.owner}, Priority: {patch.priority}");
              }
            }
          }
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        try {
          sw.AppendLine("Community Patch Information:");
          var i = 0;
          foreach (var patch in CommunityPatchSubModule.Patches) {
            var type = patch.GetType();
            sw.Append("  ").Append(++i).Append(". ").Append(type.Name);
            try {
              if (ActivePatches.ContainsKey(type))
                sw.Append(" *Active*");
              var applicability = patch.IsApplicable(Game.Current);
              if (applicability ?? false)
                sw.Append(" *Applicable*");
              if (applicability == null)
                sw.Append(" *Maybe Applicable*");
              if (patch.Applied)
                sw.Append(" *Applied*");
            }
            catch (Exception ex2) {
              sw.Append("  *** ERROR: ").Append(ex2.GetType().Name).Append(": ").AppendLine(ex2.Message);
            }

            sw.AppendLine();
          }
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        try {
          sw.AppendLine("Loaded SubModules:");
          var i = 0;
          foreach (var sm in Module.CurrentModule.SubModules) {
            try {
              var type = sm.GetType();
              var asm = type.Assembly;
              sw.Append("  ").Append(++i).Append(". ").AppendLine(type.AssemblyQualifiedName);
              foreach (var version in asm.GetCustomAttributes<AssemblyInformationalVersionAttribute>())
                sw.Append("    v").AppendLine(version.InformationalVersion);
            }
            catch (Exception ex) {
              sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
            }
          }
        }
        catch (Exception ex) {
          sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
        }

        sw.AppendLine();

        AppendSystemReport(sw);

        sw.Flush();
      }

      try {
        var now = DateTime.UtcNow;
        Random.GetNonZeroBytes(RandomIntBuf);
        var randomInt = Unsafe.ReadUnaligned<uint>(ref RandomIntBuf[0]);
        var fileName = $"diagnostic-report.{now.Year:0000}{now.Month:00}{now.Day:00}{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond}.{randomInt:X8}.txt";
        var docsMnb2 = new Uri(System.IO.Path.Combine(PathHelpers.GetConfigsDir(), "..")).LocalPath;
        var fullPath = System.IO.Path.Combine(docsMnb2, fileName);
        ShowMessage($"Saving to \"My Documents\\Mount and Blade II Bannerlord\\{fileName}\"");

        try {
          ms.Position = 0;
          using var fs = File.OpenWrite(fullPath);
          ms.WriteTo(fs);
        }
        catch (Exception ex2) {
          ShowMessage($"Failed to save diagnostic report!\n{ex2.GetType().Name}: {ex2.Message}");
        }

        try {
          ms.Position = 0;
          using var sr = new StreamReader(ms, Encoding.UTF8);
          TextCopy.Clipboard.SetText(sr.ReadToEnd());
          ShowMessage("Diagnostics also copied to system clipboard.");
        }
        catch {
          ShowMessage($"Writing report to system clipboard failed!");
          try {
            TextCopy.Clipboard.SetText(fullPath);
            ShowMessage("Writing diagnostic file path to clipboard instead.");
          }
          catch (Exception ex) {
            ShowMessage($"Writing to system clipboard failed!\n{ex.GetType().Name}: {ex.Message}");
          }
        }
      }
      catch (Exception ex) {
        ShowMessage($"Failed to diagnostic report!\n{ex.GetType().Name}: {ex.Message}");
      }
    }

    private static void AppendSystemReport(StreamWriter sw) {
      if (_systemReportCache != null) {
        sw.Flush();
        sw.BaseStream.Write(_systemReportCache, 0, _systemReportCache.Length);
        return;
      }

      sw.Flush();
      var bs = sw.BaseStream;
      var start = bs.Position;
      try {
        sw.AppendLine("System Info:");
        sw.Append("  ").AppendLine(Utilities.GetPCInfo().Replace("\n", "\n  "));
        sw.Append($"  GPU Memory: {Utilities.GetGPUMemoryMB()}MB").AppendLine();
        sw.Append($"  GC Allocated: {GC.GetTotalMemory(false)}B").AppendLine();
        sw.Append($"  Engine Memory Used: {Utilities.GetCurrentCpuMemoryUsageMB()}MB").AppendLine();
        sw.Append("  GC Latency Mode: ").Append(GCSettings.IsServerGC ? "Server " : "Client ").AppendLine(GCSettings.LatencyMode.ToString());
        sw.AppendFormat($"  GC LOH Compact Mode: {GCSettings.LargeObjectHeapCompactionMode}").AppendLine();
        sw.AppendFormat($"  Operating System: {RuntimeInformation.OSDescription}").AppendLine();
        sw.AppendFormat($"  Framework Compatibility: {RuntimeInformation.FrameworkDescription}").AppendLine();
        sw.AppendFormat($"  Hardware Accelerated Vector: {(Vector.IsHardwareAccelerated ? "Yes" : "No")}").AppendLine();
        sw.AppendFormat($"  Vector Size: {Vector<byte>.Count * 8} bits").AppendLine();
      }
      catch (Exception ex) {
        sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      try {
        var cpus = Cpu.Discover();

        sw.AppendLine("  CPU Info:");
        for (var i = 0; i < cpus.Length; ++i) {
          var cpu = cpus[i];
          cpu.Update();
          var coreCount = cpu.CoreCount;

          for (var c = 0; c < coreCount; ++c) {
            cpu.ActivateSensor(cpu.CoreClocks[c]);
            cpu.ActivateSensor(cpu.CoreTemperatures[c]);
          }

          Thread.Sleep(100);

          sw.Append("    ").Append(i + 1).Append(". ").Append(cpu.Name).Append(" with ").Append(coreCount).AppendLine(" cores:");
          for (var c = 0; c < coreCount; ++c) {
            sw.Append("      ").Append(c + 1).Append(". ")
              .Append(cpu.CoreClocks[c].Max?.ToString(CultureInfo.InvariantCulture) ?? "(?)").Append(" MHz ")
              .Append(cpu.CoreTemperatures[c].Max?.ToString(CultureInfo.InvariantCulture) ?? "(?)").Append(" °C");
            sw.AppendLine();
          }

          for (var c = 0; c < coreCount; ++c) {
            cpu.DeactivateSensor(cpu.CoreClocks[c]);
            cpu.DeactivateSensor(cpu.CoreTemperatures[c]);
          }
        }
      }
      catch (Exception ex) {
        sw.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sw.AppendLine();
      sw.Flush();
      var end = bs.Position;
      try {
        var len = end - start;
        bs.Position = start;
        var buf = new byte[len];
        bs.Read(buf, 0, buf.Length);
        _systemReportCache = buf;
      }
      catch {
        // out of memory?
        bs.Position = end;
      }
    }

  }

}