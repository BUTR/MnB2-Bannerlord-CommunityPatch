#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using Module = TaleWorlds.MountAndBlade.Module;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  [UsedImplicitly]
  [HarmonyPatch(typeof(InitialMenuVM), MethodType.Constructor)]
  public static class GroupMainMenuOptionsPatch {

    private readonly struct InitialMenuItemInfo {

      public readonly InitialStateOption Item;


      public readonly bool IsMod;

      public readonly ModuleInfo? Mod;

      public readonly MergablePropertyAttribute? Mergeable;

      public readonly string Name;

      public InitialMenuItemInfo(InitialStateOption item, bool isMod, ModuleInfo? mod, MergablePropertyAttribute? mergeable, string name) {
        Item = item;
        IsMod = isMod;
        Mod = mod;
        Mergeable = mergeable;
        Name = name;
      }

    }

    [UsedImplicitly]
    public static void Prefix() {
      if (!CommunityPatchSubModule.DontGroupThirdPartyMenuOptions)
        CleanUpMainMenu();
    }

    private const int MaxMenuLength = 8;

    private static List<InitialStateOption>? _modOptionsMenus;

    internal static List<InitialStateOption> GetThirdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(IsThirdPartyOption)
        .Concat(GroupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    private static bool _alreadyCleanedUpMainMenu;

    internal static List<InitialStateOption>? GroupedOptionsMenus;

    internal static void CleanUpMainMenu() {
      var menu = Module.CurrentModule.GetInitialStateOptions()?.ToArray()
        ?? throw new NotImplementedException("Initial state options missing!");

      var menuLength = menu.Length;

      if (_alreadyCleanedUpMainMenu && GroupedOptionsMenus != null) {
        if (menuLength <= MaxMenuLength + 1)
          return;

        var changed = false;

        for (var i = menuLength - 1; i > 0; --i) {
          var item = menu[i];

          if (!IsThirdPartyOption(item))
            continue;

          var grouped = GroupedOptionsMenus
            .FirstOrDefault(x => x.Id == item.Id);

          if (grouped == null)
            continue;

          changed = true;

          GroupedOptionsMenus.Remove(grouped);
          GroupedOptionsMenus.Add(item);
        }

        if (changed)
          SortGroupedOptionMenus();

        return;
      }

      _alreadyCleanedUpMainMenu = true;
      try {
        Array.Sort(menu, Comparer<InitialStateOption>
          .Create((a, b)
            => a.OrderIndex.CompareTo(b.OrderIndex)));
      }
      catch {
        // well whatever
      }

      if (menuLength <= MaxMenuLength)
        return;

      if (GroupedOptionsMenus != null)
        return;

      var menuItems = menu
        .Where(item => item != null)
        .Select(item => {
          var action = InitOptActGetter(item);
          var name = TextObject.ConvertToStringList(new List<TextObject> {
            item.Name
          }).FirstOrDefault() ?? item.ToString();
          if (action == null)
            return new InitialMenuItemInfo(item, false, null, MergablePropertyAttribute.No, name);
          var method = action.Method;
          var type = method.DeclaringType;
          var isMod = PathHelpers.IsModuleAssembly(type?.Assembly, out var mod);
          var mergeable = method?.GetCustomAttribute<MergablePropertyAttribute>();
          return new InitialMenuItemInfo(item, isMod, mod, mergeable, name);
        })
        .ToList();

      // because of course people pick the same names...
      var nameCollisions = menuItems
        .GroupBy(x => x.Item.Name.ToString())
        .Select(g => (Name: g.Key, Items: g.ToList()))
        .ToList();

      foreach (var (_, items) in nameCollisions) {
        if (items.Count == 1)
          continue;

        var dontMerge = new List<InitialMenuItemInfo>();
        items.RemoveAll(x => {
          var disallowed = !x.IsMergeAllowed();
          if (disallowed)
            dontMerge.Add(x);

          return disallowed;
        });

        if (items.Any(x => !x.IsMod)) {
          // 1st party wins
          items.RemoveAll(item => item.IsMod);
          items.RemoveRange(1, items.Count - 1);
          continue;
        }

        if (items.Count(x => x.IsMergeAllowed()) <= 1)
          items.RemoveAll(x => !x.IsMergeAllowed());
        else {
          var item = items.FindLast(x => x.IsMergeAllowed());
          items.Clear();
          items.Add(item);
        }

        items.AddRange(dontMerge);

        if (items.Count <= 1)
          continue;

        try {
          items.Sort((a, b) => a.Item.OrderIndex.CompareTo(b.Item.OrderIndex));
        }
        catch {
          // why you not sort
        }

        // add mod names on the end of options other than the first one
        foreach (var info in items /*.Skip(1)*/) {
          // It's better to not skip the first one, so it's clear what mod each redundantly named option belongs to
          InitOptNameSetter.Invoke(info.Item,
            new object[] {new TextObject($"{info.Name} ({info.Mod?.Name ?? "not a mod"})")});
        }
      }

      var collisionFreeMenu =
        nameCollisions
          .SelectMany(x => x.Items.Select(y => y.Item))
          .ToList();

      menu = collisionFreeMenu.ToArray();
      menuLength = menu.Length; // length needs to be updated in case collisionFreeMenu is shorter, otherwise the for loop below gets a null exception

      try {
        Array.Sort(menu, Comparer<InitialStateOption>
          .Create((a, b)
            => a.OrderIndex.CompareTo(b.OrderIndex)));
      }
      catch {
        // you'd think we'd be good by now, but ok
      }

      GroupedOptionsMenus = new List<InitialStateOption>();
      for (var i = menuLength - 1; i > 0; --i) {
        var item = menu[i];

        if (!IsThirdPartyOption(item))
          continue;

        // Always move items with name that's too wide due to collision renaming
        // onto submenu even if the menu is not at the MaxMenuLength
        if (menuLength <= MaxMenuLength && item.Name.Length <= 28)
          continue;

        GroupedOptionsMenus.Add(item);
        menu[i] = null;
        --menuLength;
      }

      SortGroupedOptionMenus();

      Module.CurrentModule.ClearStateOptions();
      foreach (var opt in menu) {
        if (opt != null)
          Module.CurrentModule.AddInitialStateOption(opt);
      }

      Module.CurrentModule.AddInitialStateOption(new InitialStateOption(
        "MoreOptions",
        new TextObject("{=MoreOptions}More Options"),
        9999, ShowMoreMainMenuOptions, false));
    }

    private static bool IsMergeAllowed(this InitialMenuItemInfo x)
      => x.Mergeable?.AllowMerge ?? false;

    private static void SortGroupedOptionMenus()
      => GroupedOptionsMenus?.Sort(Comparer<InitialStateOption>.Create((a, b) => {
        var order = a.OrderIndex.CompareTo(b.OrderIndex);
        if (order == 0)
          order = string.Compare(a.Id ?? "", b.Id ?? "", StringComparison.OrdinalIgnoreCase);
        if (order == 0)
          order = string.Compare(a.Name.ToString() ?? "", b.Name.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
        if (order == 0)
          order = a.GetHashCode().CompareTo(b.GetHashCode());
        return order;
      }));

    private static bool IsThirdPartyOption(InitialStateOption item) {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      try {
        var act = InitOptActGetter(item);
        var optAsm = act.Method.DeclaringType?.Assembly;
        var optAsmName = optAsm?.GetName().Name;
        if (optAsmName == null
          || optAsmName.StartsWith("TaleWorlds.")
          || optAsmName.StartsWith("SandBox.")
          || optAsmName.StartsWith("SandBoxCore.")
          || optAsmName.StartsWith("StoryMode."))
          if (optAsm.IsOfficialAssembly())
            return false;
      }
      catch (Exception) {
        return true;
      }

      return true;
    }

    internal static void ShowMoreMainMenuOptions()
      => ShowOptions(GroupedOptionsMenus ?? throw new NotImplementedException("Missing GroupedOptionsMenus"));

    internal static void ShowOptions(List<InitialStateOption> moreOptions) {
      if (moreOptions is null)
        throw new ArgumentNullException(nameof(moreOptions));

      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=MoreOptions}More Options").ToString(), null, moreOptions.Where(x => x != null)
        .Select(x => new InquiryElement(x.Id, x.Name.ToString(), null))
        .Where(x => !string.IsNullOrWhiteSpace(x.Title))
        .ToList(), true, true, new TextObject("{=Open}Open").ToString(), null, picked => {
        var item = picked.FirstOrDefault();
        if (item == null)
          return;

        SynchronizationContext.Current.Post(_ => {
          moreOptions.FirstOrDefault(x => string.Equals(x.Id, (string) item.Identifier, StringComparison.Ordinal))
            ?.DoAction();
        }, null);
      }, null));
    }

    private static readonly AccessTools.FieldRef<InitialStateOption, Action> InitOptActGetter = AccessTools.FieldRefAccess<InitialStateOption, Action>("_action");

    private static readonly MethodInfo InitOptNameSetter = typeof(InitialStateOption).GetMethod("set_Name", NonPublic | Instance);

  }

}