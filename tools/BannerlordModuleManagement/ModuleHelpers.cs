#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Medallion.Collections;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;

namespace BannerlordModuleManagement {

  /// <summary>
  /// Provides helpers to deal with <see cref="TaleWorlds.MountAndBlade.Module"/>s and <see cref="ModuleInfo"/>.
  /// </summary>
  public static class ModuleHelpers {

    /// <summary>
    /// Retrieves an ordered module list that includes user preferences.
    /// </summary>
    /// <param name="userData">User preference information.</param>
    /// <param name="isMultiplayer">Whether the selection is for multiplayer.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
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
        .ThenBy(mi => mi.Module.IsOfficial ? 0 : 1) // official
        .ThenBy(mi => Math.Min(mi.Module.DependedModuleIds.Count, 1)) // dep roots
        .ThenBy(mi => char.IsLetter(mi.Module.Alias[0]) || !mi.Module.IsOfficial ? 1 : 0) // prefix
        .ThenBy(mi => mi.Weight) // user input
        .Select(mi => mi.Module)
        .ToList();

      return list;
    }

    private static bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo) {
      if (isMultiplayer && moduleInfo.IsMultiplayerModule)
        return true;

      return !isMultiplayer && moduleInfo.IsSingleplayerModule;
    }

    private static readonly Dictionary<string, List<string>> ModuleOptionalDependencyCache
      = new Dictionary<string, List<string>>();

    /// <summary>
    /// Returns the DependedModulesId collection plus any optional dependencies if they apply according to selection.
    /// </summary>
    /// <param name="mi"></param>
    /// <param name="existing"></param>
    /// <returns></returns>
    public static List<string> GetDependedModuleIdsWithOptional(this ModuleInfo mi, IEnumerable<ModuleInfo> existing) {
      if (!mi.IsSelected)
        return mi.DependedModuleIds;

      try {
        if (!ModuleOptionalDependencyCache.TryGetValue(mi.Alias, out var optDepIds)) {
          var xDoc = XDocument.Load(ModuleInfo.GetPath(mi.Alias));
          optDepIds = xDoc.XPathSelectElements("/Module/OptionalDependedModules/OptionalDependedModule")
            .Select(elem => elem.Attribute("Id")?.Value)
            .Where(str => !string.IsNullOrEmpty(str))
            .ToList()!;

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

    private static readonly Regex RxModsList = new Regex(@"_MODULES_(?:([^\*]+)(?:\*|_MODULES_))*(?<=_MODULES_)",
      RegexOptions.CultureInvariant | RegexOptions.Singleline);

    /// <summary>
    /// Collects the module sequence from the command line parameters.
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<ModuleInfo>? GetModuleListFromArguments() {
      var match = RxModsList.Match(Environment.CommandLine);
      if (!match.Success)
        return null;

      var selectedIds = new HashSet<string>(match.Groups[1].Captures
        .Cast<Capture>().Select(c => c.Value));

      var mods = (IReadOnlyList<ModuleInfo>) ModuleInfo.GetModules();

      foreach (var mod in mods)
        mod.IsSelected = selectedIds.Contains(mod.Id);

      return mods;
    }

    /// <summary>
    /// A module list initialized with user selections.
    /// </summary>
    public static IReadOnlyList<ModuleInfo>? ModuleList { get; internal set; }

    /// <summary>
    /// Provides a list of modules initialized with user selections.
    /// User weighting is discarded.
    /// </summary>
    /// <returns>A list of modules.</returns>
    public static List<ModuleInfo>? GetDefaultOrderedList() {
      if (ModuleList == null) {
        ModuleList = GetModuleListFromArguments();
        if (ModuleList == null) {
          var mods = (List<ModuleInfo>) ModuleInfo.GetModules();
          if (mods == null)
            return null;

          var userDataMgr = new UserDataManager();
          userDataMgr.LoadUserData();
          var isSpAvail = mods.All(m => m.Id != "Sandbox");
          var userData = userDataMgr.UserData;
          var isMp = !isSpAvail || userData.GameType == GameType.Multiplayer;
          ModuleList = mods
            .Where(mod => IsVisible(isMp, mod))
            .Select(mod => {
              var userModData = userData.GetUserModData(isMp, mod.Id);
              if (userModData != null)
                mod.IsSelected = mod.IsSelected || userModData.IsSelected;
              return mod;
            })
            .ToList();
        }
      }

      var dict = ModuleList.ToDictionary(mi => mi.Id);

      var list = ModuleList
        .OrderTopologicallyBy(mi
          => mi.GetDependedModuleIdsWithOptional(ModuleList)
            .Select(id => dict[id]))
        .ThenBy(mi => mi.IsOfficial ? 0 : 1) // official
        .ThenBy(mi => Math.Min(mi.DependedModuleIds.Count, 1)) // dep roots
        .ThenBy(mi => char.IsLetter(mi.Alias[0]) || !mi.IsOfficial ? 1 : 0) // prefix
        .ToList();
      return list;
    }

    /// <summary>
    /// Applies stable topology sorting to the <paramref name="list"/> to avoid broken dependency orders
    /// while respecting the existing order as if it were user preference weighting or priority.
    /// </summary>
    /// <param name="list">A view model collection of module info.</param>
    public static void FixDependencyOrder(this MBBindingList<LauncherModuleVM> list) {
      var vmDict = list
        .ToDictionary(vm => vm.Info.Id);

      var copy = list.ToList();

      var sorted = copy
        .StableOrderTopologicallyBy(mod =>
          ModuleList == null
            ? mod.Info
              .DependedModuleIds
              .Select(id => vmDict[id])
            : mod.Info
              .GetDependedModuleIdsWithOptional(ModuleList)
              .Select(id => vmDict[id]))
        .ToList();

      // don't want to fire a bunch of gui update events for clear/inserts, sort is just one
      IdentitySort(list, sorted);
    }

    /// <summary>
    /// Applies stable topology sorting to the <paramref name="list"/> to avoid broken dependency orders
    /// while respecting the existing order as if it were user preference weighting or priority.
    /// </summary>
    /// <param name="list">A view model collection of module info.</param>
    public static void FixDependencyOrder(this MBBindingList<ModuleInfo> list) {
      var vmDict = list
        .ToDictionary(vm => vm.Id);

      var sorted = list
        .StableOrderTopologicallyBy(mod =>
          ModuleList == null
            ? mod
              .DependedModuleIds
              .Select(id => vmDict[id])
            : mod
              .GetDependedModuleIdsWithOptional(ModuleList)
              .Select(id => vmDict[id]))
        .ToList();

      // don't want to fire a bunch of gui update events for clear/inserts, sort is just one
      IdentitySort(list, sorted);
    }

    /// <summary>
    /// Makes the <paramref name="list"/> match the order given in <paramref name="sorted"/>.
    /// </summary>
    /// <param name="list">A collection of module info.</param>
    /// <param name="sorted">A pre-sorted list of module info.</param>
    public static void IdentitySort(this MBBindingList<ModuleInfo> list, IReadOnlyList<ModuleInfo> sorted)
      => list.Sort(Comparer<ModuleInfo>.Create((a, b)
        => sorted.FindIndex(x => x == a)
          .CompareTo(sorted.FindIndex(x => x == b))));

    /// <summary>
    /// Makes the <paramref name="list"/> match the order given in <paramref name="sorted"/>.
    /// </summary>
    /// <param name="list">A view model collection of module info.</param>
    /// <param name="sorted">A pre-sorted view model collection of module info.</param>
    private static void IdentitySort(this MBBindingList<LauncherModuleVM> list, IReadOnlyList<LauncherModuleVM> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.IndexOf(a)
          .CompareTo(sorted.IndexOf(b))));

    /// <summary>
    /// Makes the <paramref name="list"/> match the order given in <paramref name="sorted"/>.
    /// </summary>
    /// <param name="list">A view model collection of module info.</param>
    /// <param name="sorted">A pre-sorted list of module info.</param>
    public static void IdentitySort(this MBBindingList<LauncherModuleVM> list, IReadOnlyList<ModuleInfo> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => sorted.FindIndex(x => x == a.Info)
          .CompareTo(sorted.FindIndex(x => x == b.Info))));

    /// <summary>
    /// Makes the <paramref name="list"/> match the order given in <paramref name="sorted"/>.
    /// </summary>
    /// <param name="list">A view model collection of module info.</param>
    /// <param name="sorted">A pre-sorted list of identifiers.</param>
    public static void IdentitySort(this MBBindingList<LauncherModuleVM> list, IReadOnlyList<string> sorted)
      => list.Sort(Comparer<LauncherModuleVM>.Create((a, b)
        => {
        var indexA = sorted.FindIndex(x => x == a.Info.Id);
        if (indexA == -1) indexA = int.MaxValue;
        var indexB = sorted.FindIndex(x => x == b.Info.Id);
        if (indexB == -1) indexB = int.MaxValue;
        return indexA.CompareTo(indexB);
      }));

    /// <summary>
    /// Unblocks a module's assemblies.
    /// </summary>
    /// <param name="moduleInfo">The module to unblock.</param>
    public static void UnblockModule(ModuleInfo moduleInfo) {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        throw new PlatformNotSupportedException(RuntimeInformation.OSDescription);

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