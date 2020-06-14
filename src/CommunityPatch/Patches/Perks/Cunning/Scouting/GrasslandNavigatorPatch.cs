using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class GrasslandNavigatorPatch : PerkPatchBase<GrasslandNavigatorPatch>, IPartySpeed {

    public GrasslandNavigatorPatch() : base("Ekqj9IFR") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (mobileParty.IsInSnowyTerrain() || !mobileParty.IsInTerrainType(Plain) && !mobileParty.IsInTerrainType(Steppe))
        return;
      
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

  }

}