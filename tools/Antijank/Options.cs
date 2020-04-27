using System;
using System.Linq;

namespace Antijank {

  public static class Options {

    private static bool HasCommandLineArg(string argStr)
      => Environment.GetCommandLineArgs().Any(arg => arg.Equals(argStr, StringComparison.OrdinalIgnoreCase));

    public static bool EnableHarmonyDebugLogging { get; set; }
      = HasCommandLineArg("/hdiag");

    public static bool DisableFirstChanceExceptionPrinting { get; set; }
      = !HasCommandLineArg("/fce");

    public static bool EnableDiagnosticConsole
      = HasCommandLineArg("/diag");

    public static bool EnableWidgetFactoryInitializationPatch
      = HasCommandLineArg("/wfi");

  }

}