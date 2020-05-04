#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Mono.Cecil.Cil;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace CommunityPatch.Patches {

  public sealed class EarlyStoryVisibleTimeoutPatch : IPatch {

    private static readonly byte[] IsRemainingTimeHiddenGetterBodyIl = {
      // e1.1.2.226306
      // i.e. IL for "return true"
      (byte) OpCodes.Ldc_I4_1.Value, // 0x17
      (byte) OpCodes.Ret.Value, // 0x2a
    };

    private const string StoryModeAsmSpecifierSuffix = ", StoryMode, Version=1.0.0.0, Culture=neutral";

    private static readonly Type FirstPhaseType = Type.GetType("StoryMode.StoryModePhases.FirstPhase" + StoryModeAsmSpecifierSuffix, true);

    private static readonly List<MethodInfo> FirstPhaseQuestRemainingTimeHiddenGetters =
      FirstPhaseType.Assembly.GetTypes()
        .Where(type => type.Namespace == "StoryMode.Behaviors.Quests.FirstPhase" && type.IsSubclassOf(typeof(QuestBase)))
        .Append(Type.GetType("StoryMode.Behaviors.Quests.MeetWithArzagosQuestBehavior+MeetWithArzagosQuest" + StoryModeAsmSpecifierSuffix))
        .Append(Type.GetType("StoryMode.Behaviors.Quests.SupportKingdomQuestBehavior+SupportKingdomQuest" + StoryModeAsmSpecifierSuffix))
        .Select(type => {
          var getter = AccessTools.PropertyGetter(type, "IsRemainingTimeHidden");
          if (getter.IsStatic) {
            throw new InvalidOperationException("an IsRemainingTimeHidden property was not an instance property, which we require.");
          }

          return getter;
        })
        .ToList();

    private static readonly MethodInfo RemainingTimeHiddenGetterPostfixHandle = AccessTools.Method(typeof(EarlyStoryVisibleTimeoutPatch), nameof(RemainingTimeHiddenGetterPostfix));

    private static readonly Type FirstPhaseCampaignBehaviorType = Type.GetType("StoryMode.Behaviors.FirstPhaseCampaignBehavior" + StoryModeAsmSpecifierSuffix);

    private static readonly MethodInfo FirstPhaseInstanceGetter = AccessTools.PropertyGetter(FirstPhaseType, "Instance");

    private static object FirstPhaseInstance => FirstPhaseInstanceGetter.Invoke(null, null);

    private static readonly MethodInfo FirstPhaseFirstPhaseStartTimeGetter = AccessTools.PropertyGetter(FirstPhaseType, "FirstPhaseStartTime");

    private static CampaignTime GetFirstPhaseFirstPhaseStartTime(object instance)
      => (CampaignTime) FirstPhaseFirstPhaseStartTimeGetter.Invoke(instance, null);

    private static readonly Type SecondPhaseType = Type.GetType("StoryMode.StoryModePhases.SecondPhase" + StoryModeAsmSpecifierSuffix);

    private static readonly MethodInfo SecondPhaseInstanceGetter = AccessTools.PropertyGetter(SecondPhaseType, "Instance");

    private static object SecondPhaseInstance => SecondPhaseInstanceGetter.Invoke(null, null);

    private static readonly FieldInfo FirstPhaseTimeLimitInYearsField = AccessTools.Field(FirstPhaseCampaignBehaviorType, "FirstPhaseTimeLimitInYears");

    private static readonly int FirstPhaseTimeLimitInYears = ExtractFirstPhaseTimeLimitInYears();

    public bool Applied { get; private set; }

    public void Reset() {
      if (!Applied)
        return;

      CampaignEvents.OnQuestStartedEvent.ClearListeners(this);
      foreach (var method in FirstPhaseQuestRemainingTimeHiddenGetters)
        CommunityPatchSubModule.Harmony.Unpatch(method, RemainingTimeHiddenGetterPostfixHandle);

      Applied = false;
    }

    public bool? IsApplicable(Game game) {
      // if they remove the const, they probably got rid of the time limit
      if (FirstPhaseTimeLimitInYearsField == null)
        return false;

      var campaignStoryModeType = Type.GetType("StoryMode.CampaignStoryMode" + StoryModeAsmSpecifierSuffix, false);
      if (campaignStoryModeType == null)
        return false;

      if (!game.GameType.GetType().IsEquivalentTo(campaignStoryModeType))
        return false;

      if (FirstPhaseQuestRemainingTimeHiddenGetters.Count == 0)
        return false;

      foreach (var method in FirstPhaseQuestRemainingTimeHiddenGetters) {
        if (AlreadyPatchedByOthers(Harmony.GetPatchInfo(method)))
          return false;

        // if any of base game's QuestRemainingTimeHidden getters have been changed the story line
        // may have changed considerably so don't apply
        if (!method.GetMethodBody().GetILAsByteArray().SequenceEqual(IsRemainingTimeHiddenGetterBodyIl))
          return false;
      }

      return true;
    }

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

    private static int ExtractFirstPhaseTimeLimitInYears() {
      if (FirstPhaseTimeLimitInYearsField == null)
        throw new InvalidOperationException("FirstPhaseTimeLimitInYearsField was not available");

      try {
        return (int) FirstPhaseTimeLimitInYearsField
          .GetRawConstantValue();
      }
      catch {
        if (!FirstPhaseTimeLimitInYearsField.IsStatic)
          throw new InvalidOperationException("FirstPhaseTimeLimitInYears was not static and we do not have an instance");

        return (int) FirstPhaseTimeLimitInYearsField.GetValue(null);
      }
    }

    public void Apply(Game game) {
      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, OnQuestStarted);

      foreach (var quest in Campaign.Current.QuestManager.Quests.ToList())
        SetStoryVisibleTimeoutIfNeeded(quest);

      foreach (var method in FirstPhaseQuestRemainingTimeHiddenGetters)
        CommunityPatchSubModule.Harmony.Patch(method, postfix: new HarmonyMethod(RemainingTimeHiddenGetterPostfixHandle));

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

      var prevIsRemainingTimeHidden = quest.IsRemainingTimeHidden;
      quest.ChangeQuestDueTime(newDueTime);

      if (quest.IsRemainingTimeHidden && prevIsRemainingTimeHidden)
        return; // nothing we should show here

      ShowNotification(
        new TextObject("{=QuestTimeRemainingUpdated}Quest time remaining was updated."),
        "event:/ui/notification/quest_update"
      );
    }

    private static void ShowNotification(TextObject message, string soundEventPath = "")
      => InformationManager.AddQuickInformation(message, 0, null, soundEventPath);

    private void OnQuestStarted(QuestBase quest) {
      // defer our updates until some time has passed, because they depend on whether 
      // FirstPhase or SecondPhase is active, and story Phase information is only 
      // updated after all OnQuestStarted handlers have fired.
      var onHourlyTickBoxed = new StrongBox<Action>();
      onHourlyTickBoxed.Value = () => {
        SetStoryVisibleTimeoutIfNeeded(quest);
        CampaignEvents.HourlyTickEvent.ClearListeners(onHourlyTickBoxed);
      };
      CampaignEvents.HourlyTickEvent.AddNonSerializedListener(onHourlyTickBoxed, onHourlyTickBoxed.Value);
    }

    private static void RemainingTimeHiddenGetterPostfix(ref QuestBase __instance, ref bool __result)
      => __result = __instance.QuestDueTime == CampaignTime.Never;

  }

}
