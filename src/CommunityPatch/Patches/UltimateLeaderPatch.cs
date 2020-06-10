using System;
using CommunityPatch.Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Patches {

  public abstract class UltimateLeader<TPatch> : PartySizeLimitSubPatch<TPatch> where TPatch : UltimateLeader<TPatch> {

    protected UltimateLeader(Func<PerkObject, bool> perkFinder) : base(perkFinder) { }

    protected void IncreasePartySizeLimitDependingOnLeadershipSkills(ref int partySizeLimit, MobileParty party, StatExplainer explanation, int leadershipSkillStep) {

      var clanLeader = party?.LeaderHero?.Clan?.Leader;
      if (clanLeader == null)
        return;

      var leadershipValue = clanLeader.GetSkillValue(DefaultSkills.Leadership);
      if (leadershipValue <= 250)
        return;

      var perk = ActivePatch.Perk;

      if (!clanLeader.GetPerkValue(perk))
        return;

      var extra = (leadershipValue - 250) * (int) perk!.PrimaryBonus / leadershipSkillStep;

      if (extra <= 0)
        return;

      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }

  }

}