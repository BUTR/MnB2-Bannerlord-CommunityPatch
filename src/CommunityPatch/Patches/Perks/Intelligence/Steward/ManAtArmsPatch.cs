using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class ManAtArmsPatch : PartySizeLimitSubPatch<ManAtArmsPatch> {

    public ManAtArmsPatch() : base("WVLzi1fa") { }

    public override void ModifyPartySizeLimit(ref int partySizeLimit, MobileParty party, StatExplainer explanation) {

      var perk = ActivePatch.Perk;
      if (!(party.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      var fiefCount = party.LeaderHero.Clan.Settlements.Count(s => !s.IsVillage);
      var extra = fiefCount * (int) perk.PrimaryBonus;
      if (extra < 0)
        return;

      AddExplainedPartySizeLimitBonus(ref partySizeLimit, extra, explanation);
    }

  }

}