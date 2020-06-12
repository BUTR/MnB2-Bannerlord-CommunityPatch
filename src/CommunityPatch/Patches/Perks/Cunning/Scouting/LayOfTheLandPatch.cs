using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class LayOfTheLandPatch : PerkPatchBase<LayOfTheLandPatch>, IPartySpeed {

    private readonly IPartySpeed _modifyPartySpeed;

    public LayOfTheLandPatch() : base("P68GX3zY")
      => _modifyPartySpeed = new ModifyPartySpeed(Perk);

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.03f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) 
    // Party Member
      => _modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);

  }

}