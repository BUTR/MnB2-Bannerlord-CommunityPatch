using System.Linq;
using CommunityPatch.Patches;
using TaleWorlds.CampaignSystem;

namespace Patches {

  public class ModifyPartySpeed : IPartySpeed {

    private readonly PerkObject _perk;

    public ModifyPartySpeed(PerkObject perk)
      => _perk = perk;

    public void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed) {
      if (_perk == null || mobileParty == null || !mobileParty.HasPerk(_perk)) {
        return;
      }
      
      var extra = 0f;
      if (_perk.PrimaryRole == SkillEffect.PerkRole.PartyMember) {
        extra = _perk.PrimaryBonus * mobileParty.MemberRoster?.Where(x =>
            x.Character != null
            && x.Character.IsHero
            && x.Character.HeroObject != null
            && x.Character.HeroObject.GetPerkValue(_perk))
          .Count() ?? 0f;
      } else {
        extra = _perk.PrimaryBonus;
      }

      if (extra <= 0f)
        return;
      
      finalSpeed.AddFactor(extra, _perk.Name);
        
    }

  }

}