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

    private const int MaxMenuLength = 8;

    private static List<InitialStateOption> _modOptionsMenus;

    internal static List<InitialStateOption> GetThirdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(IsThirdPartyOption)
        .Concat(_groupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    private static bool _alreadyCleanedUpMainMenu;

    internal static List<InitialStateOption> _groupedOptionsMenus;

    public static void CleanUpMainMenu() {
      if (_alreadyCleanedUpMainMenu)
        return;

      _alreadyCleanedUpMainMenu = true;
      var menu = Module.CurrentModule.GetInitialStateOptions().ToArray();

      if (menu.Length <= MaxMenuLength)
        return;

      if (_groupedOptionsMenus != null)
        return;

      var menuLength = menu.Length;

      _groupedOptionsMenus = new List<InitialStateOption>();
      for (var i = menuLength - 1; i > 0; --i) {
        var item = menu[i];

        if (!IsThirdPartyOption(item))
          continue;

        if (menuLength <= MaxMenuLength)
          break;

        _groupedOptionsMenus.Add(item);
        menu[i] = null;
        --menuLength;
      }

      Module.CurrentModule.ClearStateOptions();
      foreach (var opt in menu)
        if (opt != null)
          Module.CurrentModule.AddInitialStateOption(opt);

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
          return false;
      }
      catch (Exception) {
        return true;
      }

      return true;
    }

    internal static void ShowMoreMainMenuOptions()
      => ShowOptions(_groupedOptionsMenus);

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

    private static readonly FieldInfo InitOptActField = typeof(InitialStateOption).GetField("_action", BindingFlags.NonPublic | BindingFlags.Instance);

  }

}