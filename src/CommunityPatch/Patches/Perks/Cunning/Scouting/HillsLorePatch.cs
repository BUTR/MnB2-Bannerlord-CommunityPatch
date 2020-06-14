using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class HillsLorePatch : ScoutingLorePerk<HillsLorePatch> {


    public HillsLorePatch() : base("67UGQ0Kd") { }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.05f);
      base.Apply(game);
    }

    public override void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (!mobileParty.IsInHillTerrain())
        return;
      
      base.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed); 
    }

  }

}