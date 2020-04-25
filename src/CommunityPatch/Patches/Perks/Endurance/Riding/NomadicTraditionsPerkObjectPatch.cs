using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class NomadicTraditionsPerkObjectPatch : IPatch {

    public bool Applied { get; private set; }

    private PerkObject _perk;

    public bool? IsApplicable(Game game)
      => _perk?.PrimaryBonus.Equals(0.5f);

    public void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );

      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 0.3f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );

      Applied = true;
    }

    public void Reset()
      => _perk = DefaultPerks.Riding.NomadicTraditions;

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield break;
    }

  }

}