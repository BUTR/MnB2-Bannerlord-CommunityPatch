using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace Antijank {

  [PublicAPI]
  public class AppDomainManager : System.AppDomainManager {

    private static readonly ConsoleTraceListener ConsoleTraceListener = new ConsoleTraceListener(true);

    static AppDomainManager() {
      GCSettings.LatencyMode = GCLatencyMode.LowLatency;
      ProfileOptimization.SetProfileRoot(Environment.CurrentDirectory);
      ProfileOptimization.StartProfile(nameof(Antijank) + ".profile");
    }

    [DllImport("kernel32")]
    internal static extern bool AllocConsole();

    [DllImport("kernel32")]
    internal static extern bool FreeConsole();

    [DllImport("kernel32")]
    internal static extern bool CloseHandle(IntPtr handle);

    public override void InitializeNewDomain(AppDomainSetup appDomainInfo) {
#if DEBUG
      MessageBox.Info(
        $"Antijank v{typeof(AppDomainManager).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}\n" +
        "For Mount & Blade II: Bannerlord\n" +
        "This is a non-release build.",
        "Antijank",
        help: () => {
          if (MessageBox.Info("Enable All Diagnostics?", type: MessageBoxType.YesNo) != MessageBoxResult.No) {
            Options.EnableDiagnosticConsole = true;
            Options.EnableHarmonyDebugLogging = true;
            Options.DisableFirstChanceExceptionPrinting = false;
          }
        });
#endif

      if (Options.EnableDiagnosticConsole) {
        EnableDiagnosticsConsole();
      }

      base.InitializeNewDomain(appDomainInfo);

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
            LoaderPatch.Init();
            CustomDebugManager.Init();
            TickExceptionHandler.Init();
            //MbEventExceptionHandler.Init();
            break;
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