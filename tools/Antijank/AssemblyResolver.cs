using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Win32.SafeHandles;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;
using static System.Reflection.BindingFlags;
using Module = TaleWorlds.MountAndBlade.Module;
using ModuleInfo = TaleWorlds.Library.ModuleInfo;
using Path = System.IO.Path;

namespace Antijank {

  public static class AssemblyResolver {

    private static Dictionary<string, Assembly> SafeLoadedAssemblies
      = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex RxAnythingGoes = new Regex("", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool AssemblyLoaderLoadFromPatch(out Assembly __result, string assemblyFile) {
      TryLoadAssembly(assemblyFile, out __result);

      return false;
    }

    private static bool TryLoadAssembly(string filePath, out Assembly assembly) {
      var ext = Path.GetExtension(filePath);

      if (!ext.Equals(".dll", StringComparison.OrdinalIgnoreCase)
        && !ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
        filePath += ".dll";

      if (filePath.StartsWith("../") || filePath.StartsWith("..\\") || filePath.StartsWith("./") || filePath.StartsWith(".\\"))
        filePath = new Uri(Path.Combine(Environment.CurrentDirectory, filePath)).LocalPath;

      if (!Path.IsPathRooted(filePath)) {
        var gameBinDir = PathHelpers.GetGameBinDir();
        var asmPath = Path.Combine(gameBinDir, filePath);
        if (File.Exists(asmPath))
          filePath = asmPath;
        else {
          asmPath = FindAnyModuleAssembly(filePath);
          if (asmPath != null && File.Exists(asmPath))
            filePath = asmPath;
        }
      }

      var found =
        SafeLoadedAssemblies.TryGetValue(filePath, out var safeAsm)
          ? safeAsm
          : AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(asm => !asm.IsDynamic
              && new Uri(asm.CodeBase).LocalPath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

      if (found != null) {
        // already loaded
        assembly = found;
        return false;
      }

      if (Path.IsPathRooted(filePath)) {
        if (PathHelpers.IsOfficialPath(filePath)) {
          assembly = Assembly.LoadFrom(filePath);
          return true;
        }

        assembly = SafeLoadAssembly(filePath);
        return true;
      }

      // help
      assembly = null;
      return false;
    }

    private static void RecursiveLoadReferencedAssemblies(Assembly asm) {
      foreach (var asmName in asm.GetReferencedAssemblies())
        if (TryLoadAssembly(asmName.Name, out var refdAsm))
          RecursiveLoadReferencedAssemblies(refdAsm);
    }

    private static Assembly SafeLoadAssembly(string assemblyFile) {
      try {
        var asm = Assembly.LoadFile(assemblyFile);

        try {
          RecursiveLoadReferencedAssemblies(asm);
        }
        catch (Exception) {
          Console.WriteLine("Missing some referenced assembly for " + assemblyFile);
          // warning
        }

        return asm;
      }
      catch (Exception) {
        // error
        Console.WriteLine("Failed to load assembly " + assemblyFile);
        return null;
      }
    }

    static AssemblyResolver() {
      // here comes the magic
      var twResolver = typeof(AssemblyLoader).GetMethod("OnAssemblyResolve", NonPublic | Static | DeclaredOnly);
      Context.Harmony.Patch(typeof(AssemblyLoader).GetMethod(nameof(AssemblyLoader.LoadFrom)),
        new HarmonyMethod(typeof(AssemblyResolver), nameof(AssemblyLoaderLoadFromPatch)));
      AppDomain.CurrentDomain.AssemblyResolve -= (ResolveEventHandler) Delegate.CreateDelegate(typeof(ResolveEventHandler), twResolver);

      if (AppDomain.CurrentDomain.DomainManager is AppDomainManager adm && Options.EnableDiagnosticConsole) {
        HarmonyPatch.PatchHarmonyLogging();
      }

      var domain = AppDomain.CurrentDomain;

      domain.AssemblyResolve += OnAssemblyResolve;

      domain.UnhandledException += OnUnhandledException;

      domain.FirstChanceException += OnFirstChanceException;

      domain.DomainUnload += (sender, args) => {
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
      };
    }

    private static void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs args) {
      if (Options.DisableFirstChanceExceptionPrinting)
        return;

      Console.WriteLine("First Chance Exception:");
      var ex = args.Exception;
      Logging.Log(ex);
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args) {
      if (args.IsTerminating) {
        if (Logging._terminalExceptionLoopCheck) {
          Logging._terminalExceptionLoopCheck = true;
          try {
            AppDomainManager.EnableDiagnosticsConsole();
            MessageBox.Error("Check the diagnostic console for detailed output.", "Terminal Unhandled Exception");
          }
          catch {
            // well darn
          }
        }
      }

      Console.WriteLine("Unhandled Exception:");
      var ex = args.ExceptionObject as Exception;

      Logging.Log(ex);

      try {
        var modAsm = new StackTrace(ex, false).FindModuleFromStackTrace(out var modInfo, out var stackFrame);
        Console.WriteLine();
        Console.WriteLine($"Possible Source Mod: {modInfo?.Name}");
        Console.WriteLine($"Possible Source Assembly: {modAsm?.GetName().Name}");
        Console.WriteLine($"Possible Source Call: {stackFrame?.GetMethod()?.FullDescription()}");
      }
      catch {
        // ok
      }

      if (!args.IsTerminating)
        return;

      Console.WriteLine("Exception is terminal.");
      Console.WriteLine("Press any key to exit.");
      Console.ReadKey(true);

      var hWndConsole = Kernel32.GetConsoleWindow();
      User32.SetActiveWindow(hWndConsole);
      User32.SetForegroundWindow(hWndConsole);
    }

    public static void Init() {
      // invoke static initializer
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
      try {
        var name = new AssemblyName(args.Name);

        if (name.Name == "ManagedStarter")
          return null;

        var reqAsm = args.RequestingAssembly
          ?? new StackTrace(2, false).FindModuleFromStackTrace();

        // handle ambiguous resolve requests by checking all mod bin dirs
        // TODO: recursive scan module dirs

        if (reqAsm == null) {
          var absPath = FindAnyModuleAssembly(name.Name);

          if (absPath != null)
            return SafeLoadAssembly(absPath);

          return null;
        }

        MBSubModuleBase reqSm = null;
        foreach (var sm in Module.CurrentModule.SubModules) {
          var smAsm = sm.GetType().Assembly;
          if (smAsm == reqAsm)
            reqSm = sm;
        }

        if (reqSm == null)
          return null;

        var resolvable = new LinkedList<(ModuleInfo Mod, SubModuleInfo SubMod)>();
        ModuleInfo reqMi = null;
        SubModuleInfo reqSmi = null;
        var modules = LoaderPatch.ModuleList;
        foreach (var mi in modules) {
          if (!mi.IsSelected)
            continue;

          foreach (var smi in mi.SubModules) {
            if (smi.Assemblies.Contains(args.Name))
              resolvable.AddLast((mi, smi));

            if (smi.SubModuleClassType != reqSm.GetType().FullName)
              continue;

            reqMi = mi;
            reqSmi = smi;
          }
        }

        if (reqSmi == null)
          return null;

        foreach (var modId in reqMi.DependedModuleIds) {
          foreach (var resolution in resolvable) {
            if (modId != resolution.Mod.Id)
              continue;

            var modDir = Path.GetDirectoryName(ModuleInfo.GetPath(modId));
            if (modDir == null)
              continue;

            var modPath = Path.Combine(modDir, "bin", Common.ConfigName, name.Name + ".dll");

            if (!File.Exists(modPath))
              continue;

            var absPath = new Uri(Path.Combine(Environment.CurrentDirectory, modPath)).LocalPath;

            if (SecurityHelpers.UnblockFile(absPath))
              Console.WriteLine("Unblocked: " + absPath);

            Console.WriteLine("Resolved: " + absPath);

            return SafeLoadAssembly(absPath);
          }
        }
      }
      catch {
        // TODO: log?
      }

      return null;
    }

    private static string FindAnyModuleAssembly(string name) {
      var modList = LoaderPatch.ModuleList;
      if (modList == null)
        return null;

      foreach (var modInfo in modList) {
        if (!modInfo.IsSelected)
          continue;

        var relModDir = Path.GetDirectoryName(ModuleInfo.GetPath(modInfo.Id));
        if (relModDir == null)
          continue;

        var absModDir = new Uri(Path.Combine(PathHelpers.GetGameBinDir(), relModDir)).LocalPath;

        var fileName = name;
        var ext = Path.GetExtension(name);
        if (!ext.Equals(".dll", StringComparison.OrdinalIgnoreCase)
          && !ext.Equals(".dll", StringComparison.OrdinalIgnoreCase))
          fileName += ".dll";

        var modPath = Path.Combine(absModDir, "bin", PathHelpers.GetBinSubDir(), fileName);
        if (!File.Exists(modPath))
          continue;

        var absPath = new Uri(Path.Combine(Environment.CurrentDirectory, modPath)).LocalPath;

        if (SecurityHelpers.UnblockFile(absPath))
          Console.WriteLine("Unblocked: " + absPath);

        Console.WriteLine("Resolved: " + absPath);

        return absPath;
      }

      return null;
    }

  }

}