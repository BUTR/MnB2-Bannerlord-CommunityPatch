using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher;
using TaleWorlds.MountAndBlade.Launcher.UserDatas;
using Path = System.IO.Path;

namespace Antijank {

  public static class LoaderPatch {

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    static LoaderPatch()
      => AssemblyResolver.Harmony.Patch(
        typeof(LauncherModsVM).GetMethod("LoadSubModules", Declared),
        new HarmonyMethod(typeof(LoaderPatch).GetMethod(nameof(LoadSubModulesPrefix), Declared)));

    public static void Init() {
      // static initializer
    }

    public static bool LoadSubModulesPrefix(LauncherModsVM __instance, UserData ____userData, bool isMultiplayer) {
      var list =
        ApplyUserSorting(SortByDependencies(
          ModuleInfo.GetModules()
            .Where(x => IsVisible(isMultiplayer, x))
            .Select(x => {
              var userModData = ____userData.GetUserModData(isMultiplayer, x.Id);
              if (userModData != null)
                x.IsSelected = x.IsSelected || userModData.IsSelected;
              return x;
            })
            .OrderBy(x => x.IsNative() ? 0 : x.IsOfficial ? 1 : x.IsSelected ? 2 : 3) // presort
            .ThenBy(x => Math.Min(x.DependedModuleIds.Count, 1)) // roots grouped
        ));

      foreach (var moduleInfo in list) {
        UnblockModule(moduleInfo);
        var launcherModuleVm = new LauncherModuleVM(moduleInfo,
          (targetModule, insertIndex, tag) => ChangeLoadingOrderOf(__instance, targetModule, insertIndex, tag),
          targetModule => ChangeIsSelectedOf(__instance, targetModule));
        __instance.Modules.Add(launcherModuleVm);
      }

      ModuleList = new MBReadOnlyList<ModuleInfo>(list.ToList());

      return false;
    }

    public static IReadOnlyList<ModuleInfo> ModuleList { get; private set; }

    private static void UnblockModule(ModuleInfo moduleInfo) {
      var binDir = new Uri(Path.Combine(
        Environment.CurrentDirectory,
        Path.GetDirectoryName(ModuleInfo.GetPath(moduleInfo.Alias)),
        "bin", Common.ConfigName
      )).LocalPath;

      if (!Directory.Exists(binDir))
        return;

      foreach (var file in Directory.EnumerateFiles(binDir)) {
        var ext = Path.GetExtension(file);
        if (ext == ".dll")
          if (SecurityHelpers.UnblockFile(file))
            Console.WriteLine("Unblocked: " + file);
      }
    }

    // this bit is based on @Fumblesneeze's loader
    public static LinkedList<ModuleInfo> SortByDependencies(IEnumerable<ModuleInfo> mods) {
      var unresolvedMods = new Queue<ModuleInfo>(mods);
      var addedMods = new HashSet<string>();
      var orderedModList = new LinkedList<ModuleInfo>();
      var failedCount = 0;
      while (failedCount < unresolvedMods.Count) {
        var moduleInfo = unresolvedMods.Dequeue();
        var recentlyFailed = false;
        foreach (var item in moduleInfo.DependedModuleIds) {
          if (addedMods.Contains(item))
            continue;

          unresolvedMods.Enqueue(moduleInfo);
          failedCount++;
          recentlyFailed = true;
          break;
        }

        if (recentlyFailed)
          continue;

        failedCount = 0;
        orderedModList.AddLast(moduleInfo);
        addedMods.Add(moduleInfo.Id);
      }

      foreach (var item in unresolvedMods)
        orderedModList.AddLast(item);

      return orderedModList;
    }

    public static LinkedList<ModuleInfo> ApplyUserSorting(LinkedList<ModuleInfo> mods) {
      return mods;
    }

    private static bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo) {
      if (isMultiplayer && moduleInfo.IsMultiplayerModule)
        return true;

      return !isMultiplayer && moduleInfo.IsSingleplayerModule;
    }

    // Token: 0x06000010 RID: 16 RVA: 0x00002374 File Offset: 0x00000574
    private static void ChangeLoadingOrderOf(LauncherModsVM mods, LauncherModuleVM targetModule, int insertIndex, string tag) {
      if (insertIndex >= mods.Modules.IndexOf(targetModule))
        insertIndex--;
      insertIndex = (int) MathF.Clamp(insertIndex, 0f, mods.Modules.Count - 1);
      var index = mods.Modules.IndexOf(targetModule);
      mods.Modules.RemoveAt(index);
      mods.Modules.Insert(insertIndex, targetModule);
    }

    private static void ChangeIsSelectedOf(LauncherModsVM mods, LauncherModuleVM targetModule) {
      if (targetModule.IsSelected) {
        using (var enumerator = mods.Modules.GetEnumerator()) {
          while (enumerator.MoveNext()) {
            var launcherModuleVm = enumerator.Current;
            if (launcherModuleVm == null)
              continue;

            launcherModuleVm.IsSelected |= targetModule.Info.DependedModuleIds.Contains(launcherModuleVm.Info.Id);
          }

          return;
        }
      }

      foreach (var launcherModuleVm2 in mods.Modules) {
        launcherModuleVm2.IsSelected &= !launcherModuleVm2.Info.DependedModuleIds.Contains(targetModule.Info.Id);
      }
    }

  }

}