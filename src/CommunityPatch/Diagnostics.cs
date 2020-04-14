using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HardwareProviders.CPU;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using static CommunityPatch.CommunityPatchSubModule;
using Module = TaleWorlds.MountAndBlade.Module;
using Path = TaleWorlds.Engine.Path;

namespace CommunityPatch {

  public static class Diagnostics {

    public static void CopyToClipboard() {
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
        else {
          sb.AppendLine("  Recording disabled.");
        }
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
          if (!String.IsNullOrWhiteSpace(mi.Alias))
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
        var reportStr = sb.ToString();

        try {
          Input.SetClipboardText(reportStr);
          ShowMessage("Diagnostics copied to system clipboard.");
        }
        catch (Exception ex) {
          ShowMessage($"Writing to system clipboard failed!\n{ex.GetType().Name}: {ex.Message}");
          ShowMessage("Saving to \"My Documents\\Mount and Blade II Bannerlord\\diagnostic-report.txt\"");
          try {
            var docsMnb2 = new Uri(System.IO.Path.Combine(PathHelpers.GetConfigsDir(), "..")).LocalPath;
            File.WriteAllText(System.IO.Path.Combine(docsMnb2, "diagnostic-report.txt"), reportStr);
          }
          catch (Exception ex2) {
            ShowMessage($"Failed to save diagnostic report!\n{ex2.GetType().Name}: {ex2.Message}");
          }
        }
      }
      catch (Exception ex) {
        ShowMessage($"Failed to generate string from diagnostic report string buffer!\n{ex.GetType().Name}: {ex.Message}");
      }
    }

  }

}