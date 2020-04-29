using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Medallion.Collections;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;

namespace Antijank {

  public static class Loader {

    public static List<ModuleInfo> GetOrderedModuleList(UserData userData, bool isMultiplayer) {
      var data = isMultiplayer
        ? userData.MultiplayerData
        : userData.SingleplayerData;

      var modInfos = ModuleInfo.GetModules()
        .Where(mod => IsVisible(isMultiplayer, mod))
        .Select(mod => {
          var userModData = userData.GetUserModData(isMultiplayer, mod.Id);
          if (userModData != null)
            mod.IsSelected = mod.IsSelected || userModData.IsSelected;
          return mod;
        })
        .ToList();

      var weightedModInfos = modInfos
        .Select(mod => {
          var id = mod.Id;
          var pref = data.ModDatas.FindIndex(md => md.Id == id);
          if (pref < 0) pref = int.MinValue;
          return new WeightedModuleInfo(mod, pref);
        })
        .ToList();

      var dict = weightedModInfos.ToDictionary(mi => mi.Module.Id);

      if (!modInfos.Any(mi => mi.IsSelected))
        throw new NotImplementedException();

      var list = weightedModInfos
        .OrderTopologicallyBy(mi => mi.Module
          .GetDependedModuleIdsWithOptional(modInfos)
          .Select(id => dict[id]))
        .ThenBy(mi => GetLoadGroup(mi.Module))
        .ThenBy(mi => mi.Weight) // user input
        .Select(mi => mi.Module)
        .ToList();

      return list;
    }

    private static int GetLoadGroup(ModuleInfo mi) {
      var prefixed = !char.IsLetter(mi.Alias[0]);
      var official = mi.IsOfficial;
      var root = mi.DependedModuleIds.Count == 0;
      return prefixed && root
        ? 0
        : official && root
          ? 1
          : root
            ? 2
            : official
              ? 3
              : 4;
    }

    private static bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo) {
      if (isMultiplayer && moduleInfo.IsMultiplayerModule)
        return true;

      return !isMultiplayer && moduleInfo.IsSingleplayerModule;
    }

    private static readonly Dictionary<string, List<string>> ModuleOptionalDependencyCache
      = new Dictionary<string, List<string>>();

    public static List<string> GetDependedModuleIdsWithOptional(this ModuleInfo mi, IEnumerable<ModuleInfo> existing) {
      if (!mi.IsSelected)
        return mi.DependedModuleIds;

      try {
        if (!ModuleOptionalDependencyCache.TryGetValue(mi.Alias, out var optDepIds)) {
          var xDoc = XDocument.Load(ModuleInfo.GetPath(mi.Alias));
          optDepIds = xDoc.XPathSelectElements("/Module/OptionalDependedModules/OptionalDependedModule")
            .Select(elem => elem.Attribute("Id")?.Value)
            .Where(str => !String.IsNullOrEmpty(str))
            .ToList();

          ModuleOptionalDependencyCache[mi.Alias] = optDepIds;
        }

        if (optDepIds.Count == 0)
          return mi.DependedModuleIds;

        var depModIds = mi.DependedModuleIds
          .Concat(optDepIds
            .Where(id => existing.Any(x => x.Id == id && x.IsSelected)))
          .ToList();

        return depModIds;
      }
      catch {
        // darn...
      }

      return mi.DependedModuleIds;
    }

    private static readonly Regex RxModsList = new Regex(@"_MODULES_\*(?:([^\*]+)(?:\*|\*_MODULES_))*(?<=\*_MODULES_)",
      RegexOptions.CultureInvariant | RegexOptions.Singleline);

    public static IReadOnlyList<ModuleInfo> GetModuleListFromArguments() {
      var match = RxModsList.Match(Environment.CommandLine);
      if (!match.Success)
        return null;

      var selectedIds = match.Groups[1].Captures
        .Cast<Capture>().Select(c => c.Value).ToHashSet();

      var mods = (IReadOnlyList<ModuleInfo>) ModuleInfo.GetModules();

      foreach (var mod in mods)
        mod.IsSelected = selectedIds.Contains(mod.Id);

      return mods;
    }

    public static IReadOnlyList<ModuleInfo> ModuleList { get; internal set; }

    public static List<ModuleInfo> DefaultSort() {
      var dict = ModuleList.ToDictionary(mi => mi.Id);

      var list = ModuleList
        .OrderTopologicallyBy(mi
          => mi.GetDependedModuleIdsWithOptional(ModuleList)
            .Select(id => dict[id]))
        .ThenBy(GetLoadGroup)
        .ToList();
      return list;
    }

    public static void FixSequence(MBBindingList<LauncherModuleVM> list) {
      var vmDict = list
        .ToDictionary(vm => vm.Info.Id);

      var copy = list.ToList();

      /*
      var nativeIndex = copy.FindIndex(m => m.Info.IsNative());
      var native = copy[nativeIndex];
      if (nativeIndex != 0) {
        copy.RemoveAt(nativeIndex);
        copy.Insert(0, native);
      }
      */

      var sorted = copy
        .StableOrderTopologicallyBy(mod => mod.Info
          .GetDependedModuleIdsWithOptional(ModuleList)
          .Select(id => vmDict[id]))
        .ToList();

      // don't want to fire a bunch of gui update events for clear/inserts, sort is just one
      IdentitySort(list, sorted);
    }

    private static void IdentitySort(MBBindingList<LauncherModuleVM> list, IReadOnlyList<LauncherModuleVM> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.IndexOf(a)
          .CompareTo(sorted.IndexOf(b))));

    public static void IdentitySort(MBBindingList<LauncherModuleVM> list, IReadOnlyList<ModuleInfo> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.FindIndex(x => x == a.Info)
          .CompareTo(sorted.FindIndex(x => x == b.Info))));

    public static void IdentitySort(MBBindingList<LauncherModuleVM> list, IReadOnlyList<string> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => {
        var indexA = sorted.FindIndex(x => x == a.Info.Id);
        if (indexA == -1) indexA = Int32.MaxValue;
        var indexB = sorted.FindIndex(x => x == b.Info.Id);
        if (indexB == -1) indexB = Int32.MaxValue;
        return indexA.CompareTo(indexB);
      }));

    public static void UnblockModule(ModuleInfo moduleInfo) {
      var binDir = new Uri(Path.Combine(
        Environment.CurrentDirectory,
        Path.GetDirectoryName(ModuleInfo.GetPath(moduleInfo.Alias)) ?? ".",
        "bin", Common.ConfigName
      )).LocalPath;

      if (!Directory.Exists(binDir))
        return;

      foreach (var file in Directory.EnumerateFiles(binDir)) {
        var ext = Path.GetExtension(file);
        if (ext != ".dll")
          continue;

        if (SecurityHelpers.UnblockFile(file))
          Console.WriteLine("Unblocked: " + file);
      }
    }

  }

}