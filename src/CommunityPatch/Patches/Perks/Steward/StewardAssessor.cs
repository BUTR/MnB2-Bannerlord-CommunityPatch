using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class StewardAssessorPatch : PatchBase<StewardAssessorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardAssessorPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xC8, 0x94, 0x27, 0x9C, 0xD9, 0xF0, 0x05, 0x63,
        0x1E, 0x07, 0xAB, 0x7F, 0x3A, 0x92, 0xD7, 0x25,
        0x8F, 0x6F, 0x89, 0x2E, 0xAB, 0xAB, 0x7D, 0x7D,
        0x6C, 0x5B, 0xF1, 0xAB, 0x4B, 0x89, 0x7A, 0xDA
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "xIL6vOgI");

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
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
        _perk.PrimaryRole, 0.1f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        SkillEffect.EffectIncrementType.AddFactor
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref int __result, Town town, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(town?.Governor?.GetPerkValue(perk) ?? false))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      if (explainedNumber.BaseNumber > 0) {
        explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);
        __result = (int) explainedNumber.ResultNumber;
      }
    }

  }

}