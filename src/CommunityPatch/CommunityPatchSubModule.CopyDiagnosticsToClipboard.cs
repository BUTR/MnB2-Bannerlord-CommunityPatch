using System;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using HardwareProviders.CPU;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    public static void CopyDiagnosticsToClipboard() {
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
        sb.AppendLine("Loaded SubModules:");
        var i = 0;
        foreach (var sm in Module.CurrentModule.SubModules)
          sb.Append("  ").Append(++i).Append(". ").AppendLine(sm.GetType().AssemblyQualifiedName);
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
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      try {
        var cpus = Cpu.Discover();

        sb.AppendLine("  CPU Info:");
        for (var i = 0; i < cpus.Length; ++i) {
          var cpu = cpus[i];
          var coreCount = cpu.CoreClocks.Length;
          sb.Append("    ").Append(i + 1).Append(". ").Append(cpu.Name).Append(" with ").Append(coreCount).AppendLine(" cores:");
          for (var c = 0; c < coreCount; ++c) {
            sb.Append("      ").Append(c + 1).Append(". ").Append(cpu.CoreClocks[c].Value).Append("MHz ")
              .Append(cpu.CoreTemperatures[c].Value).Append("°C");
            sb.AppendLine();
          }
        }
      }
      catch (Exception ex) {
        sb.Append("  *** ERROR: ").Append(ex.GetType().Name).Append(": ").AppendLine(ex.Message);
      }

      sb.AppendLine();

      try {
        Input.SetClipboardText(sb.ToString());
        ShowMessage("Diagnostics copied to system clipboard.");
      }
      catch (Exception ex) {
        ShowMessage($"Writing to system clipboard failed!\n{ex.GetType().Name}: {ex.Message}");
      }
    }

  }

}