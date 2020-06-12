using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public interface IDailyInfluenceGain {

    void ModifyDailyInfluenceGain(Clan clan, ref ExplainedNumber influenceChange);

  }

}