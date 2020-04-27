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
  public class GroupMainMenuOptionsPatch {

    [UsedImplicitly]
    public static void Prefix() {
      if (!CommunityPatchSubModule.DontGroupThirdPartyMenuOptions)
        CleanUpMainMenu();
    }

    private const int MaxMenuLength = 8;

    private static List<InitialStateOption> _modOptionsMenus;

    internal static List<InitialStateOption> GetThirdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(IsThirdPartyOption)
        .Concat(GroupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    private static bool _alreadyCleanedUpMainMenu;

    internal static List<InitialStateOption> GroupedOptionsMenus;

    internal static void CleanUpMainMenu() {
      if (_alreadyCleanedUpMainMenu)
        return;

      _alreadyCleanedUpMainMenu = true;
      var menu = Module.CurrentModule.GetInitialStateOptions().ToArray();
      try {
        Array.Sort(menu, Comparer<InitialStateOption>
          .Create((a, b)
            => a.OrderIndex.CompareTo(b.OrderIndex)));
      }
      catch {
        // well whatever
      }

      if (menu.Length <= MaxMenuLength)
        return;

      if (GroupedOptionsMenus != null)
        return;

      var menuLength = menu.Length;

      // ReSharper disable InconsistentNaming
      var menuItems = menu
        .Select(Item => {
          var Action = (Action) InitOptActField.GetValue(Item);
          var Method = Action.Method;
          var Type = Method.DeclaringType;
          var IsMod = PathHelpers.IsModuleAssembly(Type?.Assembly, out var Mod);
          var Mergeable = Method?.GetCustomAttribute<MergablePropertyAttribute>();
          var Name = TextObject.ConvertToStringList(new List<TextObject> {
            Item.Name
          }).FirstOrDefault();
          return (Item, Action, Method, Type, IsMod, Mod, Mergeable, Name);
        })
        .ToList();
      // ReSharper restore InconsistentNaming

      // because of course people pick the same names...
      var nameCollisions = menuItems
        .GroupBy(x => x.Item.Name.ToString())
        .Select(g => (Name: g.Key, Items: g.ToList()))
        .ToList();

      foreach (var collision in nameCollisions) {
        if (collision.Items.Count == 1)
          continue;

        var id = collision.Name;

        var dontMerge = new List<(InitialStateOption, Action, MethodInfo, Type, bool, ModuleInfo, MergablePropertyAttribute, string)>();
        collision.Items.RemoveAll(x => {
          var disallowed = !x.Mergeable?.AllowMerge ?? false;

          if (disallowed)
            dontMerge.Add(x);

          return disallowed;
        });

        if (collision.Items.Any(x => !x.IsMod)) {
          // 1st party wins
          collision.Items.RemoveAll(item => item.IsMod);
          collision.Items.RemoveRange(1, collision.Items.Count - 1);
          continue;
        }

        if (collision.Items.Count(x => x.Mergeable?.AllowMerge ?? false) <= 1)
          collision.Items.RemoveAll(x => !x.Mergeable?.AllowMerge ?? true);
        else {
          var item = collision.Items.FindLast(x => x.Mergeable?.AllowMerge ?? false);
          collision.Items.Clear();
          collision.Items.Add(item);
        }

        collision.Items.AddRange(dontMerge);

        if (collision.Items.Count <= 1)
          continue;

        try {
          collision.Items.Sort((a, b) => a.Item.OrderIndex.CompareTo(b.Item.OrderIndex));
        }
        catch {
          // why you not sort
        }

        // add mod names on the end of options other than the first one
        foreach (var info in collision.Items.Skip(1)) {
          InitOptNameSetter.Invoke(info.Item,
            new object[] {new TextObject($"{info.Name} ({info.Mod.Name})")});
        }
      }

      var collisionFreeMenu =
        nameCollisions
          .SelectMany(x => x.Items.Select(y => y.Item))
          .ToList();

      menu = collisionFreeMenu.ToArray();

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

        if (menuLength <= MaxMenuLength)
          break;

        GroupedOptionsMenus.Add(item);
        menu[i] = null;
        --menuLength;
      }

      GroupedOptionsMenus.Sort(Comparer<InitialStateOption>.Create((a, b) => {
        var order = a.OrderIndex.CompareTo(b.OrderIndex);
        if (order == 0)
          order = string.Compare((a.Id ?? ""), b.Id ?? "", StringComparison.OrdinalIgnoreCase);
        if (order == 0)
          order = string.Compare((a.Name.ToString() ?? ""), b.Name.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
        if (order == 0)
          order = a.GetHashCode().CompareTo(b.GetHashCode());
        return order;
      }));

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

    private static bool IsThirdPartyOption(InitialStateOption item) {
      try {
        var act = (Action) InitOptActField.GetValue(item);
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
      => ShowOptions(GroupedOptionsMenus);

    internal static void ShowOptions(List<InitialStateOption> moreOptions)
      => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        new TextObject("{=MoreOptions}More Options").ToString(),
        null,
        moreOptions
          .Select(x => new InquiryElement(x.Id, x.Name.ToString(), null))
          .Where(x => !string.IsNullOrWhiteSpace(x.Title))
          .ToList(),
        true,
        true,
        new TextObject("{=Open}Open").ToString(),
        null,
        picked => {
          var item = picked.FirstOrDefault();
          if (item == null)
            return;

          SynchronizationContext.Current.Post(_ => {
            moreOptions
              .FirstOrDefault(x => string.Equals(x.Id, (string) item.Identifier, StringComparison.Ordinal))
              ?.DoAction();
          }, null);
        }, null));

    private static readonly FieldInfo InitOptActField = typeof(InitialStateOption).GetField("_action", NonPublic | Instance);

    private static readonly MethodInfo InitOptNameSetter = typeof(InitialStateOption).GetMethod("set_Name", NonPublic | Instance);

  }

}