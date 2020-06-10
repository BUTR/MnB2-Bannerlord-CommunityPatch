using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class BannerlordPatch : PartySizeLimitSubPatch<BannerlordPatch> {

    public BannerlordPatch() : base("MMv0U5Yr") { }

    // ReSharper disable once InconsistentNaming

    public override void AddPartySizeLimitBonus(ref int partySizeLimit, MobileParty party, StatExplainer explanation) {
      
      var perk = ActivePatch.Perk;
      if (!(party.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      var fiefCount = party.LeaderHero.Clan.Settlements.Count(s => !s.IsVillage);
      var extra = fiefCount * (int) perk.PrimaryBonus;

      if (extra < float.Epsilon)
        return;

      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }

  }

}