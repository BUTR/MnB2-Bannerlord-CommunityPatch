using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class HealthyScoutPatch : IPatch {

    private readonly PerkObject _perk
      = PerkObject.FindFirst(x => x.Name.GetID() == "dDKOoD3e");

    public bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      => _perk.PrimaryRole == SkillEffect.PerkRole.PartyMember
        && _perk.PrimaryBonus == 0.15f;

    public void Apply(Game game) {
      // Dear TaleWorlds; Value should probably be publicly exposed, maybe by a method
      // and maybe marked [Obsolete] if you want to avoid your developers doing dirty deeds
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );

      // most of the properties of skills have private setters, yet Initialize is public
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        SkillEffect.PerkRole.Personal, 8f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
    }

  }

}