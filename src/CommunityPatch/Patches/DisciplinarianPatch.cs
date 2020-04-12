using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches {

  internal class DisciplinarianPatch : IPatch {

    public bool Applied { get; private set; }

    private PerkObject _perk;

    public bool IsApplicable(Game game)
      => _perk?.PrimaryRole == SkillEffect.PerkRole.Personal;

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
        SkillEffect.PerkRole.PartyLeader, 0f,
        _perk.PrimaryRole, _perk.PrimaryBonus,
        _perk.IncrementType
      );

      Applied = true;
    }

    public void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "ER3ieXOb");

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}