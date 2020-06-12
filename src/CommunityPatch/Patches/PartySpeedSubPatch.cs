using System.Linq;
using Patches;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {
  
  public abstract class PartySpeedSubPatch<TPatch> : PerkPatchBase<TPatch>, IPartySpeed where TPatch : PartySpeedSubPatch<TPatch> {

    private readonly IPartySpeed _modifyPartySpeed;

    protected PartySpeedSubPatch(string perkId) : base(perkId)
      => _modifyPartySpeed = new ModifyPartySpeed(Perk);

    void IPartySpeed.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (IsPerkConditionFulfilled(mobileParty, baseSpeed, finalSpeed))
        _modifyPartySpeed.ModifyFinalSpeed(mobileParty, baseSpeed, ref finalSpeed);
    }

    protected virtual bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed)
      => true;

  }
  
}
