using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

    public class ForcedMarchPatch : PerkPatchBase<ForcedMarchPatch>, IPartySpeed {

      private readonly IPartySpeed _modifyPartySpeed;

      public ForcedMarchPatch() : base("jhZe9Mfo")
        => _modifyPartySpeed = new ModifyPartySpeed(Perk);

      public override void Apply(Game game) {
        Perk.SetPrimaryBonus(0.03f);
        base.Apply(game);
      }

      public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
        if (mobileParty.Morale > 70f) {
          _modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
        }
        
      }

    }

  }

}