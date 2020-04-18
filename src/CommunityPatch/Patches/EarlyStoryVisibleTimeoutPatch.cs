using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoryMode;
using StoryMode.Behaviors;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  public sealed class EarlyStoryVisibleTimeoutPatch : IPatch {
    // default if we can't obtain vanilla's value
    private const int DefaultFirstPhaseTimeLimitInYears = 10;
    private readonly int FirstPhaseTimeLimitInYears;

    public EarlyStoryVisibleTimeoutPatch() {
      FirstPhaseTimeLimitInYears = ExtractFirstPhaseTimeLimitInYears();
    }

    public bool Applied { get; private set; }

    public void Reset() {
      if (Applied) {
        CampaignEvents.OnQuestStartedEvent.ClearListeners(this);
        Applied = false;
      }
    }

    public bool? IsApplicable(Game game) => game.GameType is StoryMode.CampaignStoryMode;

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

    private static int ExtractFirstPhaseTimeLimitInYears() {
      try {
        FieldInfo field = typeof(FirstPhaseCampaignBehavior).GetField("FirstPhaseTimeLimitInYears", BindingFlags.NonPublic | BindingFlags.Static);
        return (int)field.GetRawConstantValue();
      }
      catch (NullReferenceException ex) {
        // couldn't locate the field in vanilla. Maybe the name has changed, or it's access modifiers.
        CommunityPatchSubModule.Error(ex, $"{typeof(EarlyStoryVisibleTimeoutPatch).Name}: Couldn't locate vanilla's timeout value." + Environment.NewLine);
      }
      return DefaultFirstPhaseTimeLimitInYears;
    }

    public void Apply(Game game) {
      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(
        this,
        new Action<QuestBase>(OnQuestStarted)
      );
      foreach (QuestBase quest
               in Campaign.Current.QuestManager.Quests.ToList<QuestBase>()) {
        SetStoryVisibleTimeoutIfNeeded(quest);
      }

      Applied = true;
    }

    private static bool IsFirstStoryPhase() {
      return FirstPhase.Instance != null
               && SecondPhase.Instance == null;
    }

    private void SetStoryVisibleTimeoutIfNeeded(QuestBase quest) {
      if (!IsFirstStoryPhase() || !quest.IsSpecialQuest || !quest.IsOngoing)
        return;

      // set visible timeout to be when vanilla would have (silently) timed 
      // out the quest,  minus a day to make very sure the quest doesn't 
      // somehow trigger vanilla's silent timeout too early.
      CampaignTime newDueTime = FirstPhase.Instance.FirstPhaseStartTime
                                  + CampaignTime.Years(FirstPhaseTimeLimitInYears)
                                  - CampaignTime.Days(1);
      if (quest.QuestDueTime != newDueTime) {
        quest.ChangeQuestDueTime(newDueTime);
        ShowNotification(new TextObject("{=QuestTimeRemainingUpdated}Quest time remaining was updated."), "event:/ui/notification/quest_update");
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

  }

}
