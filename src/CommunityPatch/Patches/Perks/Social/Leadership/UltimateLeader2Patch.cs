using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Social.Leadership {
  
  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 4, 2)]
  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 4, 1)]
  public class UltimateLeader2Patch : PartySizeLimitSubPatch<UltimateLeader2Patch> {

    public UltimateLeader2Patch() : base(perk => perk.StringId == "LeadershipUltimateLeaderII") { }

    public override void ModifyPartySizeLimit(ref int partySizeLimit, MobileParty party, StatExplainer explanation) {

      var clanLeader = party?.LeaderHero?.Clan?.Leader;
      if (clanLeader == null)
        return;

      var leadershipValue = clanLeader.GetSkillValue(DefaultSkills.Leadership);
      if (leadershipValue <= 250)
        return;

      var perk = ActivePatch.Perk;

      if (!clanLeader.GetPerkValue(perk))
        return;

      var extra = (leadershipValue - 250) * (int) perk!.PrimaryBonus / 5;

      if (extra <= 0)
        return;
      
      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }

  }

}