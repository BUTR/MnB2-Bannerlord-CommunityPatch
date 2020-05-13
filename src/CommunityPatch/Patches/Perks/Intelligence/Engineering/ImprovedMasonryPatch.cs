using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class ImprovedMasonryPatch : PerkPatchBase<ImprovedMasonryPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultWallHitPointCalculationModel).GetMethod(nameof(DefaultWallHitPointCalculationModel.CalculateMaximumWallHitPoint), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ImprovedMasonryPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xD7, 0x43, 0xD4, 0x5F, 0xE4, 0xDD, 0xF1, 0x9B,
        0xF6, 0xEA, 0xD2, 0xA7, 0xBD, 0x26, 0xAC, 0xBE,
        0x50, 0x60, 0xAA, 0x21, 0x18, 0xFE, 0xFA, 0x4F,
        0xD4, 0x28, 0x10, 0xEE, 0x76, 0x6D, 0x29, 0xE8
      }
    };

    public ImprovedMasonryPatch() : base("R60kenU3") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(30f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(ref float __result, Town town, StatExplainer explanation = null) {
      var perk = ActivePatch.Perk;
      var totalHP = new ExplainedNumber(__result, explanation);

      PerkHelper.AddPerkBonusForTown(perk, town, ref totalHP);

      __result = totalHP.ResultNumber;
    }

  }

}