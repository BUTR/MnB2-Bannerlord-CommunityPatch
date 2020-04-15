using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using StoryMode;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {
    // may need to update this if vanilla's value changes.
    // vanilla has it in StoryMode.Behaviors.FirstPhaseCampaignBehavior.FirstPhaseTimeLimitInYears
    private const int FirstPhaseTimeLimitInYears = 10;

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
      if (!DontGroupThirdPartyMenuOptions)
        MenuCleaner.CleanUpMainMenu();

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

    private static bool IsFirstStoryPhase() {
      return FirstPhase.Instance != null
               && SecondPhase.Instance == null;
    }

    private void InitStoryQuestsTimeoutVisibility(Game game) {
      if (!(game.GameType is StoryMode.CampaignStoryMode))
        return;

      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(
        this,
        new Action<QuestBase>(OnQuestStarted)
      );
      foreach (QuestBase quest
               in Campaign.Current.QuestManager.Quests.ToList<QuestBase>()) {
        SetStoryVisibleTimeoutIfNeeded(quest);
      }
    }

    private static void SetStoryVisibleTimeoutIfNeeded(QuestBase quest) {
      if (!IsFirstStoryPhase() || !quest.IsSpecialQuest || !quest.IsOngoing)
        return;

      // set visible timeout to be when vanilla would have (silently) timed out the quest, 
      // minus a day to make sure the quest doesn't somehow timeout due to vanilla behavior 
      // before this.
      CampaignTime newDueTime = FirstPhase.Instance.FirstPhaseStartTime
                                  + CampaignTime.Years(FirstPhaseTimeLimitInYears)
                                  - CampaignTime.Days(1);
      if (quest.QuestDueTime != newDueTime) {
        quest.ChangeQuestDueTime(newDueTime);
        ShowNotification(new TextObject("{=!}Quest time remaining was updated."), "event:/ui/notification/quest_update");
      }
    }

    private static void ShowNotification(TextObject message, string soundEventPath = "") {
      InformationManager.AddQuickInformation(message, 0, null, soundEventPath);
    }

    class NextHourlyTickListener {
      private Action<QuestBase> _func;
      private QuestBase _quest;

      public NextHourlyTickListener(Action<QuestBase> func, QuestBase quest) {
        _func = func;
        _quest = quest;
        CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
      }

      private void OnHourlyTick() {
        _func.Invoke(_quest);
        CampaignEvents.HourlyTickEvent.ClearListeners(this);
      }
    };

    private void OnQuestStarted(QuestBase quest) {
      // defer our updates until some time has passed, because they depend on whether 
      // FirstPhase or SecondPhase is active, and story Phase information is only 
      // updated after all OnQuestStarted handlers have fired.
      new NextHourlyTickListener(new Action<QuestBase>(SetStoryVisibleTimeoutIfNeeded), quest);
    }

    public override void OnGameInitializationFinished(Game game) {
      ApplyPatches(game);
      InitStoryQuestsTimeoutVisibility(game);

      base.OnGameInitializationFinished(game);
    }

  }

}