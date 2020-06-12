using CommunityPatch.Patches;
using TaleWorlds.CampaignSystem;

namespace Patches {

  public class ModifyPartySpeed : IPartySpeed {

    private PerkObject _perk;

    public ModifyPartySpeed(PerkObject perk)
      => this._perk = perk;
    

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (_perk != null && mobileParty != null && mobileParty.HasPerk(_perk))
        finalSpeed.AddFactor(_perk.PrimaryBonus, _perk.Name);
    }

  }

}