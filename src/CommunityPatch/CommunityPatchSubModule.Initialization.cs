using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    static CommunityPatchSubModule() {
      // catch and record exceptions
      AppDomain.CurrentDomain.FirstChanceException += (sender, args) => {
        if (RecordFirstChanceExceptions)
          RecordedFirstChanceExceptions.AddLast(args.Exception);
      };
      AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
        RecordedUnhandledExceptions.AddLast((Exception) args.ExceptionObject);
        CopyDiagnosticsToClipboard();
      };

      // TODO:
      /*
      AppDomain.CurrentDomain.TypeResolve += (sender, args) => {
        return null;
      };
      */

      // help delay loaded libs refer to mods they depend on
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
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

            var modPath = Path.Combine(modDir, "bin", Common.ConfigName, args.Name + ".dll");
            if (File.Exists(modPath))
              return Assembly.LoadFile(modPath);
          }
        }

        return null;
      };
    }

    private static void LoadDelayedSubModules() {
      foreach (var mod in ModuleInfo.GetModules()) {
        var id = mod.Id;
        var subModsXmlPath = ModuleInfo.GetPath(id);
        var modDir = Path.GetDirectoryName(subModsXmlPath);
        if (modDir == null)
          continue;

        var subModsXml = new XmlDocument();
        subModsXml.Load(subModsXmlPath);
        var delayedSubMods = subModsXml.SelectNodes("/Module/DelayedSubModules/SubModule")?.OfType<XmlElement>();
        if (delayedSubMods == null)
          continue;

        var main = Module.CurrentModule;
        var typeMain = typeof(Module);

        foreach (var elem in delayedSubMods) {
          var delayedSubModInfo = new SubModuleInfo();
          delayedSubModInfo.LoadFrom(elem);

          var dllPath = Path.Combine(modDir, "bin", Common.ConfigName, delayedSubModInfo.DLLName);

          var subModAsm = AssemblyLoader.LoadFrom(dllPath);
          var subModType = subModAsm.GetType(delayedSubModInfo.SubModuleClassType);
          if (!typeof(MBSubModuleBase).IsAssignableFrom(subModType))
            continue;

          var delayLoadedSubMod = (MBSubModuleBase) subModType.InvokeMember(".ctor",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
            null, null, new object[0]);

          typeMain.InvokeMember("AddSubModule",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
            null, main, new object[] {subModAsm, delayedSubModInfo.SubModuleClassType});

          var subMods = (ICollection<MBSubModuleBase>) typeMain.InvokeMember("_submodules",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
            null, main, new object[0]);

          subMods.Add(delayLoadedSubMod);

          subModType.InvokeMember(nameof(OnSubModuleLoad),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
            null, delayLoadedSubMod, new object[0]);
        }
      }
    }

  }

}