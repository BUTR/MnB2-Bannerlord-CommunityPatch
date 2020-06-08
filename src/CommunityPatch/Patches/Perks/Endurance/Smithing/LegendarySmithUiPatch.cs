using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityPatch;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Patches.Perks.Endurance.Smithing {

  public class LegendarySmithUiPatch : PatchBase<LegendarySmithUiPatch> {

    public override bool Applied { get; protected set; }

    public override IEnumerable<MethodBase> GetMethodsChecked()
      => Enumerable.Empty<MethodBase>();

    private PerkObject _perkSiegeExpert;

    private PerkObject _perkLegendarySmith;

    public override void Reset() {
      _perkLegendarySmith = PerkObject.FindFirst(x => x.Name.GetID() == "f4lnEplc");
      _perkSiegeExpert = PerkObject.FindFirst(x => x.Name.GetID() == "I8ZZagQU");
    }

    public override bool? IsApplicable(Game game) {
      if (_perkLegendarySmith == null || _perkSiegeExpert == null) {
        return false;
      }

      if (_perkLegendarySmith.AlternativePerk == _perkSiegeExpert && _perkSiegeExpert.AlternativePerk == _perkLegendarySmith) {
        return false;
      }

      return true;
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perkLegendarySmith.Name,
          _perkLegendarySmith.Description
        }
      );

      _perkLegendarySmith.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perkLegendarySmith.Skill,
        (int) _perkLegendarySmith.RequiredSkillValue,
        _perkSiegeExpert,
        _perkLegendarySmith.PrimaryRole, _perkLegendarySmith.PrimaryBonus,
        _perkLegendarySmith.SecondaryRole, _perkLegendarySmith.SecondaryBonus,
#if AFTER_E1_4_2
        _perkLegendarySmith.PrimaryIncrementType
#else
        _perkLegendarySmith.IncrementType
#endif
      );
      Applied = true;
    }
  }
}