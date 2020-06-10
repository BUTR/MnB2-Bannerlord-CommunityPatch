using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Social.Leadership {

  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 4, 1)]
  public class UltimateLeader1Patch : UltimateLeader<UltimateLeader1Patch> {

    public UltimateLeader1Patch() : base(perk => perk.StringId == "LeadershipUltimateLeaderI") { }
    
    public override void AddPartySizeLimitBonus(ref int partySizeLimit, MobileParty party, StatExplainer explanation) =>
      IncreasePartySizeLimitDependingOnLeadershipSkills(ref partySizeLimit, party, explanation, 5);

  }

}


