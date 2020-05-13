using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class TaxCollectorPatch : PerkPatchBase<TaxCollectorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(TaxCollectorPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x52, 0xFC, 0xCB, 0x7C, 0xF9, 0x90, 0xBB, 0xAD,
        0xDE, 0xDB, 0x69, 0xE9, 0x9A, 0x41, 0x28, 0x04,
        0x7C, 0xE9, 0xDE, 0xBB, 0xD5, 0xD0, 0xCA, 0x3B,
        0x4B, 0xE3, 0x19, 0x42, 0x1E, 0x36, 0xC8, 0xB2
      },
      new byte[] {
        // e1.4.0.228531
        0x3D, 0xAD, 0xE7, 0x5E, 0xA9, 0xF2, 0xDA, 0x3B,
        0x32, 0x20, 0x39, 0xEE, 0x6A, 0xA6, 0xCA, 0x70,
        0x5E, 0xB5, 0x37, 0xC1, 0x52, 0xC9, 0xFB, 0x1B,
        0x5B, 0x59, 0xFF, 0x88, 0x14, 0xC2, 0x5F, 0xB6
      }
    };

    public TaxCollectorPatch() : base("UbxBYp60") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref int __result, Town town, StatExplainer explanation) {
      var perk = ActivePatch.Perk;
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