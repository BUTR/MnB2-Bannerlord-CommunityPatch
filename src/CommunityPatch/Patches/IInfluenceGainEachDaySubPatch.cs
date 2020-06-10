using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public interface IInfluenceGainEachDaySubPatch : IPatch {

    void AddInfluenceGainBonus(Clan clan, ref ExplainedNumber influenceChange);

  }

}