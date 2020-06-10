using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public interface IPartySizeLimitSubPatch : IPatch {

    void AddPartySizeLimitBonus(ref int partySizeLimit, MobileParty party, StatExplainer explanation);

  }

}