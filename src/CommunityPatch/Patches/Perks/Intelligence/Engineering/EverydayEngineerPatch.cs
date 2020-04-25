using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class EverydayEngineerPatch : PatchBase<EverydayEngineerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultBuildingEffectModel).GetMethod(nameof(DefaultBuildingEffectModel.GetBuildingEffectAmount), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(EverydayEngineerPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xAA, 0x75, 0x66, 0xFC, 0x8F, 0x81, 0x1E, 0xB2,
        0x03, 0x56, 0xE5, 0x77, 0x6D, 0x67, 0x38, 0x45,
        0xBF, 0xF7, 0xA3, 0x85, 0x5C, 0x30, 0x6E, 0x7C,
        0xF6, 0xE8, 0x90, 0xEF, 0x25, 0x04, 0x32, 0x28
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "wwuuplH7");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus.IsDifferentFrom(.3f)) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

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

      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 60f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref float __result, Building building) {
      if (!building.BuildingType.IsDefaultProject) return;
      if (building.Town == null) return;

      var perk = ActivePatch._perk;
      var totalEffect = new ExplainedNumber(__result);
      PerkHelper.AddPerkBonusForTown(perk, building.Town, ref totalEffect);
      __result = totalEffect.ResultNumber;
    }

  }

}