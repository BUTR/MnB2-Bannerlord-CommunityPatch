using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private static readonly MethodInfo PatchMethodInfo = typeof(PeakFormPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private readonly PerkObject _perk
      = PerkObject.FindFirst(x => x.Name.GetID() == "fBgGbxaw");

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk.PrimaryBonus != 0f)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xb4, 0x8e, 0x91, 0x0e, 0x9f, 0x71, 0xc0, 0xf8,
        0x15, 0xa7, 0x63, 0xb0, 0x0b, 0x56, 0x76, 0xd2,
        0x02, 0xb5, 0xa7, 0x61, 0x3a, 0x52, 0x58, 0x23,
        0x5e, 0xbf, 0x3f, 0xb6, 0xe4, 0x93, 0xee, 0xa1
      });
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

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    public static void Postfix(ref int __result, CharacterObject character, StatExplainer explanation) {
      var result = __result;

      var explainedNumber = new ExplainedNumber(result, explanation, null);

      var perk = ActivePatch._perk;

      PerkHelper.AddPerkBonusForCharacter(perk, character, ref explainedNumber);

      __result = MBMath.Round(explainedNumber.ResultNumber);
    }

  }

}