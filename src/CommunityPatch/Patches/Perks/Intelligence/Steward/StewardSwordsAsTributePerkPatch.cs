using System;
using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class SwordsAsTributePatch : PartySizeLimitSubPatch<SwordsAsTributePatch> {

    public SwordsAsTributePatch() : base("7fHHThQr") { }

    public override void ModifyPartySizeLimit(ref int partySizeLimit, MobileParty party, StatExplainer explanation) {

      var perk = ActivePatch.Perk;
      var hero = party.LeaderHero;

      if (hero == null || hero.Clan?.Kingdom?.RulingClan?.Leader != hero)
        return;

      if (!hero.GetPerkValue(perk))
        return;

      var kingdomClans = hero.Clan?.Kingdom?.Clans;
      if (kingdomClans == null)
        return;

      var kingdomVassals = kingdomClans.Count(kingdomClan => !kingdomClan.IsUnderMercenaryService) - 1;
      var extra = (int) Math.Max(0, kingdomVassals * perk.PrimaryBonus);
      if (extra <= 0)
        return;

      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }

  }

}