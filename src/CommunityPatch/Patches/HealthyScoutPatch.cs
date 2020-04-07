using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;
using Harmony = HarmonyLib.Harmony;

namespace CommunityPatch.Patches {

  internal class HealthyScoutPatch : IPatch {

    public bool Applied { get; private set; }

    private readonly PerkObject _perk
      = PerkObject.FindFirst(x => x.Name.GetID() == "dDKOoD3e");

    private static readonly Type CharacterStatsModelType = typeof(CharacterStatsModel);

    private static readonly Type DefaultCharacterStatsModelType = typeof(DefaultCharacterStatsModel);

    private CharacterStatsModel _modelToWrap;

    private WrapperCharacterStatsModel _wrapped;

    private class WrapperCharacterStatsModel : CharacterStatsModel {

      private CharacterStatsModel _wrapped;

      private HealthyScoutPatch _patch;

      public WrapperCharacterStatsModel(CharacterStatsModel wrapped, HealthyScoutPatch patch) {
        _wrapped = wrapped;
        _patch = patch;
      }

      public override int MaxHitpoints(CharacterObject character, StatExplainer explanation = null) {
        var result = _wrapped.MaxHitpoints(character, explanation);

        var explainedNumber = new ExplainedNumber(result, explanation, null);

        PerkHelper.AddPerkBonusForCharacter(_patch._perk, character, ref explainedNumber);

        return MBMath.Round(explainedNumber.ResultNumber);
      }

    }

    public bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (!(_perk.PrimaryRole == SkillEffect.PerkRole.PartyMember
        && _perk.PrimaryBonus == 0.15f))
        return false;

      var models = (List<GameModel>) CommunityPatchSubModule.CampaignGameStarter.Models;
      _modelToWrap = (CharacterStatsModel) models.Find(model => CharacterStatsModelType.IsInstanceOfType(model));
      var modelToWrapType = _modelToWrap.GetType();

      // check if model is already non-default
      return modelToWrapType == DefaultCharacterStatsModelType
        || (modelToWrapType.Namespace?.StartsWith(nameof(CommunityPatch)) ?? false);
    }

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

      var models = (List<GameModel>) CommunityPatchSubModule.CampaignGameStarter.Models;

      _wrapped = new WrapperCharacterStatsModel(_modelToWrap, this);

      models.Remove(_modelToWrap);
      models.Add(_wrapped);

      Applied = true;
    }

  }

}