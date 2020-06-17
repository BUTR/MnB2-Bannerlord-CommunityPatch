using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class HillsLorePatch : ScoutingLorePerk<HillsLorePatch> {


    public HillsLorePatch() : base("67UGQ0Kd") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    protected override bool IsIncreasedPartySpeedPerkConditionFulfilled(MobileParty mobileParty)
      => mobileParty.IsInHillTerrain();

    protected override bool IsHalvedFoodConsumptionPerkConditionFulfilled(MobileParty mobileParty)
      => true;

  }

}