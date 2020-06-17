using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class DesertLorePatch : ScoutingLorePerk<DesertLorePatch> {

    public DesertLorePatch() : base("EeQv1qRD") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }
    
    protected override bool IsIncreasedPartySpeedPerkConditionFulfilled(MobileParty mobileParty)
      => mobileParty.IsInTerrainType(Desert);

    protected override bool IsHalvedFoodConsumptionPerkConditionFulfilled(MobileParty mobileParty)
      => true;

  }

}