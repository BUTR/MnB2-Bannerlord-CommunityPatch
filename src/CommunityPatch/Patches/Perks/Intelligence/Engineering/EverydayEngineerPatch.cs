using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class EverydayEngineerPatch : PerkPatchBase<EverydayEngineerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultBuildingEffectModel).GetMethod(nameof(DefaultBuildingEffectModel.GetBuildingEffectAmount), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(EverydayEngineerPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xAA, 0x75, 0x66, 0xFC, 0x8F, 0x81, 0x1E, 0xB2,
        0x03, 0x56, 0xE5, 0x77, 0x6D, 0x67, 0x38, 0x45,
        0xBF, 0xF7, 0xA3, 0x85, 0x5C, 0x30, 0x6E, 0x7C,
        0xF6, 0xE8, 0x90, 0xEF, 0x25, 0x04, 0x32, 0x28
      },
      new byte[] {
        // e1.4.0.228531
        0xFA, 0x46, 0xE6, 0x5B, 0x59, 0x1B, 0x0B, 0x72,
        0xA6, 0x0C, 0x72, 0x6A, 0xFB, 0xDF, 0x82, 0xD0,
        0x21, 0x30, 0x1A, 0xBC, 0x5F, 0x78, 0x09, 0xD8,
        0x67, 0xDF, 0xE9, 0x11, 0xEC, 0x83, 0x20, 0x45
      },
      new byte[] {
        // e1.4.1.230527
        0x4A, 0x8C, 0x72, 0xC8, 0x88, 0x1D, 0x01, 0x39,
        0x4B, 0xEE, 0x1A, 0xFA, 0x0A, 0x4E, 0x8A, 0x3D,
        0xB5, 0x4A, 0xFB, 0x21, 0x33, 0xA7, 0xEC, 0xE2,
        0x4D, 0x90, 0x78, 0x3F, 0xF8, 0xE5, 0xAB, 0x81
      },
      new byte[] {
        // e1.4.2.231233
        0x46, 0xDD, 0x95, 0xB4, 0x25, 0xB9, 0x76, 0x1D,
        0xAE, 0xC3, 0x6B, 0xD7, 0x12, 0xFC, 0x4A, 0x45,
        0xFE, 0x84, 0x2C, 0x37, 0x85, 0x1F, 0xCF, 0x63,
        0xF2, 0xBD, 0xD4, 0x96, 0x31, 0x87, 0x9A, 0x78
      }
    };

    public EverydayEngineerPatch() : base("wwuuplH7") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus.IsDifferentFrom(.3f)) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(60f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    public static void Postfix(ref float __result, Building building) {
      if (!building.BuildingType.IsDefaultProject) return;
      if (building.Town == null) return;

      var perk = ActivePatch.Perk;
      var totalEffect = new ExplainedNumber(__result);
      PerkHelper.AddPerkBonusForTown(perk, building.Town, ref totalEffect);
      __result = totalEffect.ResultNumber;
    }

  }

}