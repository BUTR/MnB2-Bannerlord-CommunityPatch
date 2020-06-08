using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches {
  public abstract class PartySpeedSubPatch<TPatch> : PerkPatchBase<TPatch>, IPartySpeedSubPatch where TPatch : PartySpeedSubPatch<TPatch> {
    protected PartySpeedSubPatch(string perkId) : base(perkId) { }

    void IPartySpeedSubPatch.ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed)
      => finalSpeed.AddFactor(Perk.PrimaryBonus, Perk.Name);
  }
}
