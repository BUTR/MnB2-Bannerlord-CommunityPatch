using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  public static class MenuCleaner {

    private const int MaxMenuLength = 9;

    private static List<InitialStateOption> _modOptionsMenus;

    internal static List<InitialStateOption> GetThirdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(IsThirdPartyOption)
        .Concat(_groupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    private static bool _alreadyCleanedUpMainMenu = false;

    internal static List<InitialStateOption> _groupedOptionsMenus;

    public static void CleanUpMainMenu() {
      if (_alreadyCleanedUpMainMenu)
        return;

      _alreadyCleanedUpMainMenu = true;
      var menu = Module.CurrentModule.GetInitialStateOptions().ToArray();
      if (menu.Length > MaxMenuLength) {
        if (_groupedOptionsMenus != null)
          return;

        var skippedFirst = false;
        _groupedOptionsMenus = new List<InitialStateOption>();
        for (var i = 0; i < menu.Length; i++) {
          var item = menu[i];
          if (!IsThirdPartyOption(item))
            continue;

          if (!skippedFirst) {
            skippedFirst = true;
            continue;
          }

          _groupedOptionsMenus.Add(item);
          menu[i] = null;
        }

        Module.CurrentModule.ClearStateOptions();
        foreach (var opt in menu)
          if (opt != null)
            Module.CurrentModule.AddInitialStateOption(opt);

        Module.CurrentModule.AddInitialStateOption(new InitialStateOption(
          "MoreOptions",
          new TextObject("{=MoreOptions}More Options"),
          10001, ShowMoreMainMenuOptions, false));
      }
    }

    private static bool IsThirdPartyOption(InitialStateOption item) {
      var act = (Action) InitOptActField.GetValue(item);
      var optAsm = act.Method.DeclaringType?.Assembly;
      var optAsmName = optAsm?.GetName().Name;
      if (optAsmName == null)
        return false;
      if (optAsmName.StartsWith("TaleWorlds."))
        return false;
      if (optAsmName.StartsWith("SandBox."))
        return false;
      if (optAsmName.StartsWith("SandBoxCore."))
        return false;
      if (optAsmName.StartsWith("StoryMode."))
        return false;

      return true;
    }

    internal static void ShowMoreMainMenuOptions()
      => ShowOptions(_groupedOptionsMenus);

    internal static void ShowOptions(List<InitialStateOption> moreOptions)
      => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        new TextObject("{=MoreOptions}More Options").ToString(),
        null,
        moreOptions.Select(x => new InquiryElement(x.Id, x.Name.ToString(), null)
          )
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

    private static readonly FieldInfo InitOptActField = typeof(InitialStateOption).GetField("_action", BindingFlags.NonPublic | BindingFlags.Instance);

  }

}