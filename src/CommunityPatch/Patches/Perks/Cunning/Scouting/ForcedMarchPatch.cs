using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public class ForcedMarchPatch : PerkPatchBase<ForcedMarchPatch>, IPartySpeed {

    public ForcedMarchPatch() : base("jhZe9Mfo") { }
    
    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(0.03f);
      base.Apply(game);
    }

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (mobileParty.Morale <= 70f)
        return;
      
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
    }

  }

}