using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class MarshesLorePatch : ScoutingLorePerk<MarshesLorePatch> {

    public MarshesLorePatch() : base("ywmjNJnH") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    protected override bool IsIncreasedPartySpeedPerkConditionFulfilled(MobileParty mobileParty)
      => mobileParty.IsInTerrainType(Swamp) || mobileParty.IsInTerrainType(ShallowRiver);

    protected override bool IsHalvedFoodConsumptionPerkConditionFulfilled(MobileParty mobileParty)
      => true;

  }

}