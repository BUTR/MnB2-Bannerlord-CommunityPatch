using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

    internal static CommunityPatchSubModule Current
      => Module.CurrentModule.SubModules.OfType<CommunityPatchSubModule>().FirstOrDefault();

    [PublicAPI]
    internal static CampaignGameStarter CampaignGameStarter;

    protected override void OnBeforeInitialModuleScreenSetAsRoot() {
      var module = Module.CurrentModule;

      {
        // remove the space option that DeveloperConsole module adds
        var spaceOpt = module.GetInitialStateOptionWithId("Space");
        if (spaceOpt != null) {
          var opts = module.GetInitialStateOptions()
            .Where(opt => opt != spaceOpt).ToArray();
          module.ClearStateOptions();
          foreach (var opt in opts)
            module.AddInitialStateOption(opt);
        }
      }

      CleanUpMainMenu();

      if (DisableIntroVideo) {
        try {
          typeof(Module)
            .GetField("_splashScreenPlayed", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(module, true);
        }
        catch (Exception ex) {
          Error(ex, "Couldn't disable intro video.");
        }
      }

      base.OnBeforeInitialModuleScreenSetAsRoot();
    }

    private List<InitialStateOption> _modOptionsMenus;

    internal List<InitialStateOption> Get3rdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(Is3rdPartyOption)
        .Concat(_groupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    internal List<InitialStateOption> _groupedOptionsMenus;

    private void CleanUpMainMenu() {
      var menu = Module.CurrentModule.GetInitialStateOptions().ToArray();
      if (menu.Length > 8) {
        if (_groupedOptionsMenus != null)
          return;

        _groupedOptionsMenus = new List<InitialStateOption>();
        for (var i = 0; i < menu.Length; i++) {
          var item = menu[i];
          if (!Is3rdPartyOption(item))
            continue;

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
          9998, ShowMoreMainMenuOptions, false));
      }
    }

    private static bool Is3rdPartyOption(InitialStateOption item) {
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

    internal void ShowMoreMainMenuOptions()
      => ShowOptions(_groupedOptionsMenus);

    internal void ShowMore3rdPartyOptions()
      => ShowOptions(Get3rdPartyOptionsMenus());

    internal void ShowOptions(List<InitialStateOption> moreOptions)
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

    protected override void OnSubModuleLoad() {
      var module = Module.CurrentModule;

      try {
        Harmony.PatchAll(typeof(CommunityPatchSubModule).Assembly);
      }
      catch (Exception ex) {
        Error(ex, "Could not apply all generic attribute-based harmony patches.");
      }

      module.AddInitialStateOption(new InitialStateOption(
        "CommunityPatchOptions",
        new TextObject("{=CommunityPatchOptions}Community Patch Options"),
        9998,
        ShowOptions,
        false
      ));

      base.OnSubModuleLoad();
    }

    internal static void ShowMessage(string msg) {
      InformationManager.DisplayMessage(new InformationMessage(msg));
      Print(msg);
    }

    public override void OnCampaignStart(Game game, object starterObject) {
      if (starterObject is CampaignGameStarter cgs)
        CampaignGameStarter = cgs;

      base.OnCampaignStart(game, starterObject);
    }

    public override void OnGameInitializationFinished(Game game) {
      ApplyPatches(game);

      base.OnGameInitializationFinished(game);
    }

  }

  [HarmonyPatch(typeof(EscapeMenuVM), MethodType.Constructor, typeof(IEnumerable<EscapeMenuItemVM>), typeof(TextObject))]
  public static class GroupEscapeMenuOptionsPatch {

    public static FieldInfo EscapeMenuItemVmOnExecute = typeof(EscapeMenuItemVM)
      .GetField("_onExecute", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public static FieldInfo EscapeMenuItemVmIdentifier = typeof(EscapeMenuItemVM)
      .GetField("_identifier", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly object _groupEscMenuOptsKey = new object();

    public static void Postfix(EscapeMenuVM __instance, ref MBBindingList<EscapeMenuItemVM> ____menuItems, IEnumerable<EscapeMenuItemVM> items, TextObject title = null) {
      var list = ____menuItems.ToList();

      var groupedOptionsMenus = new List<EscapeMenuItemVM>();
      for (var i = 0; i < list.Count; i++) {
        var item = list[i];

        var act = (Action<object>) EscapeMenuItemVmOnExecute.GetValue(item);
        var actAsm = act.Method.DeclaringType?.Assembly;
        var optAsmName = actAsm?.GetName().Name;

        if (optAsmName == null)
          continue;
        if (optAsmName.StartsWith("TaleWorlds."))
          continue;
        if (optAsmName.StartsWith("SandBox."))
          continue;
        if (optAsmName.StartsWith("SandBoxCore."))
          continue;
        if (optAsmName.StartsWith("StoryMode."))
          continue;

        groupedOptionsMenus.Add(item);
        list[i] = null;
      }

      var newList = new MBBindingList<EscapeMenuItemVM>();

      foreach (var item in list) {
        if (item != null)
          newList.Add(item);
      }

      if (groupedOptionsMenus.Count == 0) {
        newList.Add(new EscapeMenuItemVM(new TextObject("{=MoreOptions}More Options"),
          _ => CommunityPatchSubModule.Current.ShowMore3rdPartyOptions(),
          _groupEscMenuOptsKey, false));
        ____menuItems = newList;
        return;
      }

      newList.Add(new EscapeMenuItemVM(new TextObject("{=MoreOptions}More Options"), _ => {
        var options = new List<InquiryElement> {
          new InquiryElement(_groupEscMenuOptsKey, new TextObject("{=MoreOptions}More Options").ToString(), null)
        };

        foreach (var item in groupedOptionsMenus) {
          var id = EscapeMenuItemVmIdentifier.GetValue(item);
          options.Add(new InquiryElement(id, item.ActionText, null, !item.IsDisabled, null));
        }

        InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
          new TextObject("{=MoreOptions}More Options").ToString(),
          null,
          options,
          true,
          true,
          new TextObject("{=Open}Open").ToString(),
          null,
          selection => {
            var picked = selection.FirstOrDefault()?.Identifier;
            if (picked == _groupEscMenuOptsKey) {
              CommunityPatchSubModule.Current.ShowMore3rdPartyOptions();
              return;
            }

            foreach (var item in groupedOptionsMenus) {
              var id = EscapeMenuItemVmIdentifier.GetValue(item);
              if (picked != id)
                continue;

              item.ExecuteAction();
              return;
            }
          },
          null
        ), true);
      }, "MoreOptions", false));

      ____menuItems = newList;
    }

  }

}