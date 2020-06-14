using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {
  
  public abstract class PartySpeedSubPatch<TPatch> : PerkPatchBase<TPatch>, IPartySpeed where TPatch : PartySpeedSubPatch<TPatch> {

    protected PartySpeedSubPatch(string perkId) : base(perkId) { }

    void IPartySpeed.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      IPartySpeed modifyPartySpeed = new ModifyPartySpeed(Perk);
      
      if (IsPerkConditionFulfilled(mobileParty, baseSpeed, finalSpeed))
        modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
    }

    protected virtual bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed)
      => true;

  }
  
}
