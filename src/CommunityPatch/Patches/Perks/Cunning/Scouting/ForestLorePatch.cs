using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.TerrainType;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class ForestLorePatch : ScoutingLorePerk<ForestLorePatch> {

    public ForestLorePatch() : base("TgOwisdD") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    public override void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (!mobileParty.IsInTerrainType(Forest))
        return;
      
      base.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

  }

}