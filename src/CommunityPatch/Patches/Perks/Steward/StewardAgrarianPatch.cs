using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class StewardAgrarianPatch : PatchBase<StewardAgrarianPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultVillageProductionCalculatorModel).GetMethod(nameof(DefaultVillageProductionCalculatorModel.CalculateDailyFoodProductionAmount), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardAgrarianPatch).GetMethod("Postfix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xAF, 0x2E, 0xAE, 0x5B, 0x32, 0x0E, 0x01, 0x5B,
        0xAA, 0x0D, 0x9D, 0x79, 0x07, 0x1E, 0xB1, 0xCC,
        0x4E, 0xCC, 0xAD, 0xDB, 0xFA, 0x07, 0xF3, 0x54,
        0x34, 0x5C, 0xF2, 0xD2, 0x77, 0x69, 0x87, 0x93
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "XNc2NIGL");

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
        _perk.PrimaryRole, 0.3f,
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
    public static void Postfix(ref float __result, Village village) {
      var perk = ActivePatch._perk;
      if (!(village.Bound?.Town?.Governor?.GetPerkValue(perk) ?? false)) {
        return;
      }

      __result += (int) (__result * perk.PrimaryBonus);
    }

  }

}