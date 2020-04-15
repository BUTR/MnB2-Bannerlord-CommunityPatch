using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public abstract class SaveIdTypeMissingQuest : QuestBase {

    protected SaveIdTypeMissingQuest(string questId, Hero questGiver, CampaignTime duration, int rewardGold, string saveId)
      : base(questId, questGiver, duration, rewardGold) {
      var msg = FormattableString.Invariant($"Missing SaveId {saveId} for Quest: {questId}.");

      Title = new TextObject($"<{msg}>");
    }

    protected override void SetDialogs() {
    }

    protected override void InitializeQuestOnGameLoad() {
    }

    public override TextObject Title { get; }

    public override bool IsRemainingTimeHidden => false;

  }

}