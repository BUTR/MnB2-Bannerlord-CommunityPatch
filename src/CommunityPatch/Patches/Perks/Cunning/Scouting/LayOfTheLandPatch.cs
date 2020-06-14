using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class LayOfTheLandPatch : PerkPatchBase<LayOfTheLandPatch>, IPartySpeed {

    public LayOfTheLandPatch() : base("P68GX3zY") { }
    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.03f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
    }

  }

}