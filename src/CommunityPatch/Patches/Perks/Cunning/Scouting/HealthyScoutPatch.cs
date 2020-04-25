using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public sealed class HealthyScoutPatch : PatchBase<HealthyScoutPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultCharacterStatsModel)
      .GetMethod(nameof(DefaultCharacterStatsModel.MaxHitpoints), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(HealthyScoutPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x92, 0xC3, 0x48, 0x33, 0x3C, 0x1A, 0x39, 0x52,
        0xE5, 0x8B, 0xE4, 0x3E, 0xBD, 0x86, 0xFC, 0xA7,
        0x56, 0x27, 0xD9, 0x3A, 0xA5, 0x53, 0xC4, 0xDF,
        0xE2, 0x7B, 0x97, 0xBC, 0xB1, 0xAE, 0x36, 0x5E
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "dDKOoD3e");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      try {
        if (!(_perk.PrimaryRole == SkillEffect.PerkRole.PartyMember
          && _perk.PrimaryBonus == 0.15f))
          return null;
      }
      catch (Exception) {
        return null;
      }

      return true;
    }

    public override void Apply(Game game) {
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

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref int __result, CharacterObject character, StatExplainer explanation) {
      var result = __result;

      var explainedNumber = new ExplainedNumber(result, explanation);

      var perk = ActivePatch._perk;

      PerkHelper.AddPerkBonusForCharacter(perk, character, ref explainedNumber);

      __result = MBMath.Round(explainedNumber.ResultNumber);
    }

  }

}