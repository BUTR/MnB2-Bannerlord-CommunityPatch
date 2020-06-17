using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class GrasslandNavigatorPatch : PartySpeedSubPatch<GrasslandNavigatorPatch> {

    public GrasslandNavigatorPatch() : base("Ekqj9IFR") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    protected override bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed) 
      => !mobileParty.IsInSnowyTerrain() && mobileParty.IsInTerrainType(Plain) || mobileParty.IsInTerrainType(Steppe);
    
  }

}