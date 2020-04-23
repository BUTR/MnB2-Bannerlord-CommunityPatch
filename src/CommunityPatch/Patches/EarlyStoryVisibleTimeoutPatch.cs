using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches {

  public sealed class EarlyStoryVisibleTimeoutPatch : IPatch {

    private static readonly Type FirstPhaseCampaignBehaviorType = Type.GetType("StoryMode.Behaviors.FirstPhaseCampaignBehavior, StoryMode, Version=1.0.0.0, Culture=neutral");

    private static readonly Type FirstPhaseType = Type.GetType("StoryMode.StoryModePhases.FirstPhase, StoryMode, Version=1.0.0.0, Culture=neutral");

    private static readonly MethodInfo FirstPhaseInstanceGetter = AccessTools.PropertyGetter(FirstPhaseType, "Instance");

    private static object FirstPhaseInstance => FirstPhaseInstanceGetter.Invoke(null, null);

    private static readonly MethodInfo FirstPhaseFirstPhaseStartTimeGetter = AccessTools.PropertyGetter(FirstPhaseType, "FirstPhaseStartTime");

    private static CampaignTime GetFirstPhaseFirstPhaseStartTime(object instance) => (CampaignTime) FirstPhaseFirstPhaseStartTimeGetter.Invoke(instance, null);

    private static readonly Type SecondPhaseType = Type.GetType("StoryMode.StoryModePhases.SecondPhase, StoryMode, Version=1.0.0.0, Culture=neutral");

    private static readonly MethodInfo SecondPhaseInstanceGetter = AccessTools.PropertyGetter(SecondPhaseType, "Instance");

    private static object SecondPhaseInstance => SecondPhaseInstanceGetter.Invoke(null, null);

    private static readonly int FirstPhaseTimeLimitInYears = ExtractFirstPhaseTimeLimitInYears();

    private static readonly FieldInfo FirstPhaseTimeLimitInYearsField = AccessTools.Field(FirstPhaseCampaignBehaviorType, "FirstPhaseTimeLimitInYears");

    public bool Applied { get; private set; }

    public void Reset() {
      if (!Applied)
        return;

      CampaignEvents.OnQuestStartedEvent.ClearListeners(this);
      Applied = false;
    }

    // if they remove the const, they probably got rid of the time limit
    public bool? IsApplicable(Game game)
      => FirstPhaseTimeLimitInYearsField != null
        && game.GameType.GetType().FullName == "StoryMode.CampaignStoryMode";

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

    private static int ExtractFirstPhaseTimeLimitInYears() {
      if (FirstPhaseTimeLimitInYearsField == null)
        return 0;

      try {
        return (int) FirstPhaseTimeLimitInYearsField
          .GetRawConstantValue();
      }
      catch {
        return (int) FirstPhaseTimeLimitInYearsField
          .GetValue(FirstPhaseTimeLimitInYearsField.IsStatic ? null : FirstPhaseInstance);
      }
    }

    public void Apply(Game game) {
      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, OnQuestStarted);

      foreach (var quest in Campaign.Current.QuestManager.Quests.ToList())
        SetStoryVisibleTimeoutIfNeeded(quest);

      Applied = true;
    }

    private static bool IsFirstStoryPhase()
      => FirstPhaseInstance != null
        && SecondPhaseInstance == null;

    private static void SetStoryVisibleTimeoutIfNeeded(QuestBase quest) {
      if (!IsFirstStoryPhase() || !quest.IsSpecialQuest || !quest.IsOngoing)
        return;

      // set visible timeout to be when vanilla would have (silently) timed 
      // out the quest,  minus a day to make very sure the quest doesn't 
      // somehow trigger vanilla's silent timeout too early.
      var newDueTime = GetFirstPhaseFirstPhaseStartTime(FirstPhaseInstance)
        + CampaignTime.Years(FirstPhaseTimeLimitInYears)
        - CampaignTime.Days(1);
      if (quest.QuestDueTime == newDueTime)
        return;

      quest.ChangeQuestDueTime(newDueTime);
      ShowNotification(
        new TextObject("{=QuestTimeRemainingUpdated}Quest time remaining was updated."),
        "event:/ui/notification/quest_update"
      );
    }

    private static void ShowNotification(TextObject message, string soundEventPath = "")
      => InformationManager.AddQuickInformation(message, 0, null, soundEventPath);

    private void OnQuestStarted(QuestBase quest) {
      // give OnHourlyTick's closure a late bound self-ref
      var onHourlyTickBoxed = new StrongBox<Action>();

      // defer our updates until some time has passed, because they depend on whether 
      // FirstPhase or SecondPhase is active, and story Phase information is only 
      // updated after all OnQuestStarted handlers have fired.
      onHourlyTickBoxed.Value = () => {
        SetStoryVisibleTimeoutIfNeeded(quest);
        CampaignEvents.HourlyTickEvent.ClearListeners(onHourlyTickBoxed.Value);
      };

      CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, onHourlyTickBoxed.Value);
    }

  }

}