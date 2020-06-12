using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public interface IPartySizeLimit {

    void ModifyPartySizeLimit(ref int partySizeLimit, MobileParty party, StatExplainer explanation);

  }

}