using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;
using ModuleInfo = TaleWorlds.Library.ModuleInfo;

namespace Antijank {

  public static class AssemblyResolver {

    public static readonly Harmony Harmony = new Harmony(nameof(Antijank));

    static AssemblyResolver() {
      var domain = AppDomain.CurrentDomain;

      domain.AssemblyResolve += OnResolve;

      domain.UnhandledException += (sender, args) => {
        Console.WriteLine("Unhandled Exception:");
        var ex = args.ExceptionObject as Exception;

        Log(ex);

        if (!args.IsTerminating)
          return;

        Console.WriteLine("Exception is terminal.");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
      };

      domain.FirstChanceException += (sender, args) => {
        if (DisableFirstChanceExceptionPrinting)
          return;

        Console.WriteLine("First Chance Exception:");
        var ex = args.Exception;
        Log(ex);
      };

      domain.DomainUnload += (sender, args) => {
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
      };
    }

    public static bool DisableFirstChanceExceptionPrinting { get; set; }
      = !Environment.GetCommandLineArgs().Any(arg => arg.Equals("/fce", StringComparison.OrdinalIgnoreCase));

    private static void Log(Exception ex) {
      while (ex != null) {
        Console.WriteLine(ex.ToString());

        if (ex is ReflectionTypeLoadException rtl) {
          foreach (var lex in rtl.LoaderExceptions)
            Log(lex);
        }
        else if (ex is AggregateException aex) {
          foreach (var cex in aex.InnerExceptions)
            Log(cex);
          return;
        }

        ex = ex.InnerException;
      }
    }

    public static void Init() {
      // invoke static initializer
    }

    private static Assembly OnResolve(object sender, ResolveEventArgs args) {
      try {
        var name = new AssemblyName(args.Name);

        // handle ambiguous resolve requests by checking all mod bin dirs
        // TODO: recursive scan module dirs
        if (args.RequestingAssembly == null) {
          foreach (var modInfo in ModuleInfo.GetModules()) {
            if (!modInfo.IsSelected)
              continue;

            var modDir = Path.GetDirectoryName(ModuleInfo.GetPath(modInfo.Id));
            if (modDir == null)
              continue;

            var modPath = Path.Combine(modDir, "bin", Common.ConfigName, name.Name + ".dll");
            if (!File.Exists(modPath))
              continue;

            var absPath = new Uri(Path.Combine(Environment.CurrentDirectory, modPath)).LocalPath;

            if (SecurityHelpers.UnblockFile(absPath))
              Console.WriteLine("Unblocked: " + absPath);

            Console.WriteLine("Resolved: " + absPath);

            return Assembly.LoadFrom(absPath);
          }

          return null;
        }

        MBSubModuleBase reqSm = null;
        foreach (var sm in Module.CurrentModule.SubModules) {
          var smAsm = sm.GetType().Assembly;
          if (smAsm == args.RequestingAssembly)
            reqSm = sm;
        }

        if (reqSm == null)
          return null;

        var resolvable = new LinkedList<(ModuleInfo Mod, SubModuleInfo SubMod)>();
        ModuleInfo reqMi = null;
        SubModuleInfo reqSmi = null;
        var modules = ModuleInfo.GetModules();
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

            return Assembly.LoadFrom(absPath);
          }
        }
      }
      catch {
        // TODO: log?
      }

      return null;
    }

  }

}