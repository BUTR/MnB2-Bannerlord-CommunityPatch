using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

      var depSet = modInfos.Select(mi => mi.Id).ToHashSet();

      var weightedModInfos = modInfos
        .Select(mod => {
          var id = mod.Id;
          var missingDepIds = mod.DependedModuleIds.Where(x => !depSet.Contains(x)).ToHashSet();
          if (missingDepIds.Count > 0) {
            MessageBox.Warning($"{mod.Name} is missing dependencies:\n{string.Join(Environment.NewLine, missingDepIds)}");
            return default;
          }

          var pref = data.ModDatas.FindIndex(md => md.Id == id);
          if (pref < 0) pref = int.MinValue;
          return new WeightedModuleInfo(mod, pref);
        })
        .Where(mod => mod != default)
        .ToList();

      var weightedDict = weightedModInfos.ToDictionary(mi => mi.Module.Id);

      if (!modInfos.Any(mi => mi.IsSelected))
        throw new NotImplementedException();

      var list = weightedModInfos
        .OrderTopologicallyBy(
          mi => mi.Module
            .GetDependedModuleIds(modInfos)
            .Select(id => weightedDict.TryGetValue(id, out var dep) ? dep : throw new KeyNotFoundException($"Can't find Dependency {id} for {mi.Module.Name}"))
        )
        .ThenBy(mi => mi.Weight) // user input
        .Select(mi => mi.Module)
        .ToList();

      return list;
    }

    public static int GetLoadGroup(ModuleInfo mi) {
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

    private static readonly Dictionary<string, IList<string>> ModuleLoadAfterCache
      = new Dictionary<string, IList<string>>();

    private static readonly Dictionary<string, IList<string>> ModuleOptionalDependencyCache
      = new Dictionary<string, IList<string>>();

    public static List<string> GetDependedModuleIds(this ModuleInfo mi, IEnumerable<ModuleInfo> existing) {
      IList<string> loadAfterIds = null;
      try {
        if (!ModuleLoadAfterCache.TryGetValue(mi.Alias, out loadAfterIds)) {
          var xDoc = LoadSubModuleXmlCached(mi);
          loadAfterIds = xDoc
            .XPathSelectElements("/Module/LoadAfterModules/LoadAfterModule")
            .Select(elem => elem.Attribute("Id")?.Value)
            .Where(str => !string.IsNullOrEmpty(str))
            .ToList();

          ModuleLoadAfterCache[mi.Alias] = loadAfterIds;
        }

        // must be after cache
        loadAfterIds = loadAfterIds.Where(id => existing.Any(x => x.Id == id && x.IsSelected)).ToList();
      }
      catch {
        // TODO: report error
      }

      loadAfterIds ??= Array.Empty<string>();

      IList<string> optDepIds = null;

      // the key difference
      if (mi.IsSelected) {
        try {
          if (!ModuleOptionalDependencyCache.TryGetValue(mi.Alias, out optDepIds)) {
            var xDoc = LoadSubModuleXmlCached(mi);
            optDepIds = xDoc
              .XPathSelectElements("/Module/OptionalDependedModules/OptionalDependedModule")
              .Select(elem => elem.Attribute("Id")?.Value)
              .Where(str => !string.IsNullOrEmpty(str))
              .ToList();

            ModuleOptionalDependencyCache[mi.Alias] = optDepIds;
          }

          // must be after cache
          optDepIds = optDepIds.Where(id => existing.Any(x => x.Id == id && x.IsSelected)).ToList();
        }
        catch {
          // TODO: report error
        }
      }

      optDepIds ??= Array.Empty<string>();

      try {
        var depModIds = mi.DependedModuleIds
          .Concat(loadAfterIds)
          .Concat(optDepIds)
          .Distinct()
          .ToList();

        return depModIds;
      }
      catch {
        // TODO: report error
      }

      return mi.DependedModuleIds;
    }

    private static readonly Dictionary<string, XDocument> SubModuleXmlCache
      = new Dictionary<string, XDocument>();

    public static XDocument LoadSubModuleXmlCached(ModuleInfo mi) {
      var alias = mi.Alias;
      if (SubModuleXmlCache.TryGetValue(alias, out var xDoc))
        return xDoc;

      xDoc = XDocument.Load(ModuleInfo.GetPath(alias));
      SubModuleXmlCache.Add(alias, xDoc);

      return xDoc;
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
      var idConflicts = ModuleList.GroupBy(mi => mi.Id).Where(g => g.Count() > 1).ToList();
      if (idConflicts.Count > 0) {
        var sb = new StringBuilder("Conflicting Module Ids:\n\n");
        foreach (var g in idConflicts) {
          sb.AppendLine(g.Key);
          foreach (var mi in g) {
            sb.Append(" - ").AppendLine(mi.Name);
          }

          sb.AppendLine();
        }

        MessageBox.Error(sb.ToString());
      }

      var dict = ModuleList.ToDictionary(mi => mi.Id);
      var missingDeps = new HashSet<ModuleInfo>();
      var list = ModuleList
        .Where(mod => {
          var hasNoMissingDeps = mod.DependedModuleIds.All(id => dict.ContainsKey(id));
          if (!hasNoMissingDeps)
            missingDeps.Add(mod);
          return hasNoMissingDeps;
        })
        .OrderBy(GetLoadGroup)
        .ThenBy(mi => mi.Alias)
        .OrderTopologicallyBy(mi
          => mi.GetDependedModuleIds(ModuleList)
            .Select(id => dict.TryGetValue(id, out var dep) ? dep : throw new KeyNotFoundException($"Can't find Dependency {id} for {mi.Name}")))
        .ThenBy(GetLoadGroup)
        .ThenBy(mi => mi.IsSelected ? 0 : 1)
        .ToList();

      var missing = ModuleList.ToHashSet();
      missing.ExceptWith(list);

      if (missingDeps.Count > 0) {
        var sb = new StringBuilder("Mods missing dependencies:\n");
        foreach (var mod in missing) {
          mod.IsSelected = false;
          list.Add(mod);
          sb.Append(mod.Name).Append(": ").AppendLine(string.Join(", ", mod.DependedModuleIds.Where(id => !dict.ContainsKey(id))));
        }

        MessageBox.Warning(sb.ToString());
      }

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
          .GetDependedModuleIds(ModuleList)
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
        if (indexA == -1) indexA = int.MaxValue;
        var indexB = sorted.FindIndex(x => x == b.Info.Id);
        if (indexB == -1) indexB = int.MaxValue;
        return indexA.CompareTo(indexB);
      }));

    public static void IdentitySort(List<ModuleInfo> list, IReadOnlyList<string> sorted)
      => list.Sort(Comparer<ModuleInfo>.Create((a, b)
        => {
        var indexA = sorted.FindIndex(x => x == a.Id);
        if (indexA == -1) indexA = int.MaxValue;
        var indexB = sorted.FindIndex(x => x == b.Id);
        if (indexB == -1) indexB = int.MaxValue;
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