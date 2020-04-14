using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

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

      CleanUpManuMenu(module);

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

    private void CleanUpManuMenu(Module module) {
      var menu = module.GetInitialStateOptions().ToArray();
      if (menu.Length > 8) {
        _groupedOptionsMenus = new List<InitialStateOption>();
        for (var i = 0; i < menu.Length; i++) {
          var item = menu[i];
          var act = (Action) InitOptActField.GetValue(item);
          var optAsm = act.Method.DeclaringType.Assembly;
          var optAsmName = optAsm.GetName().Name;
          if (optAsmName.StartsWith("TaleWorlds."))
            continue;
          if (optAsmName.StartsWith("SandBox."))
            continue;
          if (optAsmName.StartsWith("SandBoxCore."))
            continue;
          if (optAsmName.StartsWith("StoryMode."))
            continue;

          _groupedOptionsMenus.Add(item);
          menu[i] = null;
        }

        module.ClearStateOptions();
        foreach (var opt in menu)
          if (opt != null)
            module.AddInitialStateOption(opt);

        module.AddInitialStateOption(new InitialStateOption(
          "MoreOptions",
          new TextObject("{=MoreOptions}More Options"),
          9998, () => {
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
              new TextObject("{=MoreOptions}More Options").ToString(),
              null,
              _groupedOptionsMenus
                .Select(x => new InquiryElement(x.Id, x.Name.ToString(), null))
                .ToList(),
              true,
              true,
              new TextObject("{=Open}Open").ToString(),
              null,
              picked => {
                var item = picked.FirstOrDefault();
                if (item == null)
                  return;

                _groupedOptionsMenus
                  .FirstOrDefault(x => string.Equals(x.Id, (string) item.Identifier, StringComparison.Ordinal))
                  ?.DoAction();
              },
              null));
          }, false));
      }
    }

    internal List<InitialStateOption> _groupedOptionsMenus;

    private static readonly FieldInfo InitOptActField = typeof(InitialStateOption).GetField("_action", BindingFlags.NonPublic | BindingFlags.Instance);

    internal static void ShowGroupedOptionsMenus() {
    }

    protected override void OnSubModuleLoad() {
      var module = Module.CurrentModule;

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

}