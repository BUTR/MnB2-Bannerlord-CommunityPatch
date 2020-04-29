using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using Antijank.Debugging;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;
using Debugger = System.Diagnostics.Debugger;

namespace Antijank {

  public class AppDomainManager : System.AppDomainManager {

    private static readonly ConsoleTraceListener ConsoleTraceListener = new ConsoleTraceListener(true);

    [DllImport("kernel32")]
    internal static extern bool AllocConsole();

    [DllImport("kernel32")]
    internal static extern bool FreeConsole();

    [DllImport("kernel32")]
    internal static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32")]
    internal static extern bool SetCurrentDirectory(string path);

    static AppDomainManager() {
      var ownAsm = typeof(AppDomainManager).Assembly;
      var ownAsmPath = new Uri(ownAsm.CodeBase).LocalPath;
      var profilerAsmPath = Path.Combine(Path.GetDirectoryName(ownAsmPath)!, "AntijankProfiler.dll");

      if (Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING") != "1"
        && (!Debugger.IsAttached
          || MessageBox.Warning(
            "To properly debug with Antijank, you need the following environment variables set:\n\n"
            + "COR_ENABLE_PROFILING=1\n\n"
            + "COR_PROFILER={204AEE3C-CEE0-43D7-BFCA-6B0AFB22F09C}\n\n"
            + "COR_PROFILER_PATH=profilerAsmPath\n\n"
            + "Most debuggers provide a way to set environment variables as part of the launch configuration.\n"
            + "Do you wish to continue without Antijank's profiler?\n"
            + "Hitting no will continue with Antijank's profiler but may detach your debugger.",
            "Debugging Warning",
            MessageBoxType.YesNo
          ) == MessageBoxResult.No)) {
        Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "1");
        Environment.SetEnvironmentVariable("COR_PROFILER", "{204AEE3C-CEE0-43D7-BFCA-6B0AFB22F09C}");
        Environment.SetEnvironmentVariable("COR_PROFILER_PATH", profilerAsmPath);

        string entryExeFileName;
        using (var proc = Process.GetCurrentProcess())
          entryExeFileName = proc.MainModule?.FileName;

        var procStartInfo = new ProcessStartInfo(entryExeFileName, Environment.CommandLine) {
          UseShellExecute = false
        };

        foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
          procStartInfo.Environment.Add(envVar.Key.ToString(), envVar.Value.ToString());

        using var newProc = Process.Start(procStartInfo);
        newProc!.WaitForExit();
        Environment.Exit(newProc.ExitCode);
        return;
      }

      GCSettings.LatencyMode = GCLatencyMode.LowLatency;
      var cwd = Path.GetDirectoryName(ownAsmPath);
      SetCurrentDirectory(cwd);
      ProfileOptimization.SetProfileRoot(Environment.CurrentDirectory);
      ProfileOptimization.StartProfile(nameof(Antijank) + ".profile");
    }

    public override void InitializeNewDomain(AppDomainSetup appDomainInfo) {
      if (!Options.DisableStartUpDialog) {
        MessageBox.Info(
          $"Antijank v{typeof(AppDomainManager).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}\n" +
          "For Mount & Blade II: Bannerlord\n" +
#if DEBUG
          "This is a non-release build.",
#else
          "This is a release build.",
#endif
          "Antijank",
          help: () => {
            if (MessageBox.Info("Enable All Workarounds?", type: MessageBoxType.YesNo) != MessageBoxResult.No) {
              Options.EnableWidgetFactoryInitializationPatch = true;
              Options.EnableObjectManagerXmlLoaderPatch = true;
              Options.EnableFixMissingProperties = true;
              Options.EnableModuleFileScanningPatch = true;
            }

            if (MessageBox.Info("Enable Full Diagnostic Suite?", type: MessageBoxType.YesNo) != MessageBoxResult.No) {
              Options.EnableDiagnosticConsole = true;
              Options.EnableHarmonyDebugLogging = true;
              Options.DisableFirstChanceExceptionPrinting = false;
            }
          });
      }

      if (Options.EnableDiagnosticConsole) {
        EnableDiagnosticsConsole();
      }

      base.InitializeNewDomain(appDomainInfo);

      AppDomain.CurrentDomain.TypeResolve += (sender, args) => {
        if (args.Name.StartsWith("TaleWorlds."))
          Debugger.Break();
        return null;
      };

      AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => {
        Console.Write("Loaded ");
        Console.WriteLine(args.LoadedAssembly.GetName().ToString());

        var name = args.LoadedAssembly.GetName().Name;
        if (name != "TaleWorlds.MountAndBlade.Launcher"
          && name != "TaleWorlds.Library"
          && name != "ManagedStarter")
          return;

        AssemblyResolver.Init();

        switch (name) {
          case "TaleWorlds.MountAndBlade.Launcher":
            try {
              LoaderPatch.Init();
            }
            catch (Exception ex) {
              Console.WriteLine("LoaderPatch failed to initialize.");
              Console.WriteLine(ex);
            }

            try {
              CustomDebugManager.Init();
            }
            catch (Exception ex) {
              Console.WriteLine("CustomDebugManager failed to initialize.");
              Console.WriteLine(ex);
            }

            if (Options.EnableWidgetFactoryInitializationPatch)
              try {
                WidgetFactoryPatch.Init();
              }
              catch (Exception ex) {
                Console.WriteLine("WidgetFactoryPatch failed to initialize.");
                Console.WriteLine(ex);
              }

            //TickExceptionHandler.Init();
            //MbEventExceptionHandler.Init();
            break;
          case "ManagedStarter": {
            if (Options.EnableWidgetFactoryInitializationPatch)
              WidgetFactoryPatch.Init();
            if (Options.EnableObjectManagerXmlLoaderPatch)
              MbObjectManagerPatch.Init();
            if (Options.EnableFixMissingProperties)
              FixMissingPropertiesPatches.Init();
            break;
          }
        }
      };
    }

    public static void EnableDiagnosticsConsole() {
      if (AllocConsole())
        Console.WriteLine("Diagnostic console allocated.");
      Trace.Listeners.Add(ConsoleTraceListener);
      User32.ReleaseCapture();
      var hWndConsole = Kernel32.GetConsoleWindow();
      User32.SetActiveWindow(hWndConsole);
      User32.SetForegroundWindow(hWndConsole);
      User32.SetForegroundWindow(hWndConsole);
    }

    public static void DisableDiagnosticsConsole() {
      FreeConsole();
      Trace.Listeners.Remove(ConsoleTraceListener);
    }

    public override HostExecutionContextManager HostExecutionContextManager
      => WarpManager.Instance;

  }

}