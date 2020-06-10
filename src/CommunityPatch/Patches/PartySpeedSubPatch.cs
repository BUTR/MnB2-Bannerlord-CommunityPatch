using System.Linq;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {
  
  public abstract class PartySpeedSubPatch<TPatch> : PerkPatchBase<TPatch>, IPartySpeedSubPatch where TPatch : PartySpeedSubPatch<TPatch> {
    
    protected PartySpeedSubPatch(string perkId) : base(perkId) { }

    void IPartySpeedSubPatch.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (IsPerkConditionFulfilled(mobileParty, baseSpeed, finalSpeed) && mobileParty.HasPerk(Perk))
        finalSpeed.AddFactor(Perk.PrimaryBonus, Perk.Name);
    }

    protected virtual bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed)
      => true;

  }
  
}
