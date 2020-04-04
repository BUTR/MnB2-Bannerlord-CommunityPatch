using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class PeakFormPatch : IPatch {

    private readonly PerkObject _perk
      = PerkObject.FindFirst(x => x.Name.GetID() == "fBgGbxaw");

    public bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      => _perk.PrimaryBonus == 0f;

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
        _perk.PrimaryRole, 10f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
    }

  }

}