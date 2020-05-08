using System;
using System.Linq;
using System.Reflection;
using CommunityPatch.Behaviors;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

    internal static CommunityPatchSubModule Current => Module.CurrentModule.SubModules.OfType<CommunityPatchSubModule>().FirstOrDefault();

    [PublicAPI]
    internal static CampaignGameStarter CampaignGameStarter;

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

      if (DisableIntroVideo)
        try {
          typeof(Module)
            .GetField("_splashScreenPlayed", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(module, true);
        }
        catch (Exception ex) {
          Error(ex, "Couldn't disable intro video.");
        }

      base.OnBeforeInitialModuleScreenSetAsRoot();
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

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
      if (game.GameType is Campaign) {
        var cgs = (CampaignGameStarter) gameStarterObject;
        cgs.AddBehavior(new CommunityPatchCampaignBehavior());
      }
    }

  }

}