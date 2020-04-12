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

namespace CommunityPatch.Patches {

  internal class PeakFormPatch : PatchBase<PeakFormPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultCharacterStatsModel).GetMethod(nameof(DefaultCharacterStatsModel.MaxHitpoints), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(PeakFormPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x15, 0x64, 0x14, 0x75, 0x80, 0xB5, 0xA5, 0x46,
        0x42, 0x64, 0x27, 0x7F, 0x0A, 0x09, 0x13, 0xCC,
        0x6E, 0xDC, 0x3A, 0x06, 0xB5, 0xC7, 0xAE, 0x18,
        0x61, 0x8C, 0x19, 0x1D, 0x79, 0x81, 0x61, 0xD5
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "fBgGbxaw");

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null)
        return false;
      if (_perk.PrimaryBonus != 0f)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
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
        _perk.PrimaryRole, 10f,
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

      var explainedNumber = new ExplainedNumber(result, explanation, null);

      var perk = ActivePatch._perk;

      PerkHelper.AddPerkBonusForCharacter(perk, character, ref explainedNumber);

      __result = MBMath.Round(explainedNumber.ResultNumber);
    }

  }

}