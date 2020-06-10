using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 4, 1)]
  public class UltimateLeader2Patch : UltimateLeader<UltimateLeader2Patch> {

    public UltimateLeader2Patch() : base(perk => perk.StringId == "LeadershipUltimateLeaderII") { }

    public override void AddPartySizeLimitBonus(ref int partySizeLimit, MobileParty party, StatExplainer explanation) =>
      IncreasePartySizeLimitDependingOnLeadershipSkills(ref partySizeLimit, party, explanation, 1);

  }

}