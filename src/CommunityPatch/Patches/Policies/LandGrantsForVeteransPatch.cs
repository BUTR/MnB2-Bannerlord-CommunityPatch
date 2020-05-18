using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Policies {

  public sealed class LandGrantsForVeteransPatch : PatchBase<LandGrantsForVeteransPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo1 =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo TargetMethodInfo2 =
      typeof(DefaultSettlementMilitiaModel).GetMethod("CalculateMilitiaSpawnRate", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1Prefix =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Prefix1), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1Postfix =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Postfix1), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Postfix2), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo1;
      yield return TargetMethodInfo2;
    }

    public static readonly byte[][] Hashes1 = {
      new byte[] {
        // CalculateDailyTaxInternal e1.0.10
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

    public static readonly byte[][] Hashes2 = {
      new byte[] {
        // CalculateMilitiaSpawnRate e1.0.10
        0xE4, 0x38, 0xF5, 0x91, 0x73, 0x90, 0x52, 0x1E,
        0xCF, 0xC5, 0xE4, 0xCE, 0x3B, 0xA6, 0x3E, 0xFD,
        0x41, 0x26, 0x50, 0xDD, 0x95, 0xC8, 0xC0, 0x86,
        0x81, 0x4C, 0x02, 0xC8, 0xA9, 0xDA, 0xCA, 0xBE
      },
      new byte[] {
        // e1.4.1.229326
        0xAF, 0x6A, 0xC1, 0xCE, 0x83, 0x1F, 0xB6, 0x1A,
        0x8D, 0x1F, 0xE9, 0x46, 0x50, 0xF7, 0x1E, 0xFD,
        0x5E, 0x68, 0x3F, 0x7D, 0x69, 0x23, 0x6F, 0x3A,
        0xF8, 0xAF, 0x18, 0x87, 0x7F, 0xB4, 0x8B, 0x08
      }
    };

    public override void Reset() {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo1,
        new HarmonyMethod(PatchMethodInfo1Prefix),
        new HarmonyMethod(PatchMethodInfo1Postfix));

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo2,
        postfix: new HarmonyMethod(PatchMethodInfo2));

      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo1 = Harmony.GetPatchInfo(TargetMethodInfo1);
      if (AlreadyPatchedByOthers(patchInfo1))
        return false;

      var hash1 = TargetMethodInfo1.MakeCilSignatureSha256();
      if (!hash1.MatchesAnySha256(Hashes1))
        return false;

      var patchInfo2 = Harmony.GetPatchInfo(TargetMethodInfo2);
      if (AlreadyPatchedByOthers(patchInfo2))
        return false;

      var hash2 = TargetMethodInfo2.MakeCilSignatureSha256();
      return hash2.MatchesAnySha256(Hashes2);
    }

    // ReSharper disable once InconsistentNaming

    private static void Prefix1(Town town, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming

    private static void Postfix1(ref int __result, Town town, StatExplainer explanation) {
      var kingdom = town.Owner?.Settlement?.OwnerClan?.Kingdom;
      if (kingdom == null)
        return;

      if (!kingdom.ActivePolicies.Contains(DefaultPolicies.LandGrantsForVeterans))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      if (explainedNumber.BaseNumber < float.Epsilon)
        return;

      explainedNumber.AddFactor(-0.05f, DefaultPolicies.LandGrantsForVeterans.Name);
      __result = (int) explainedNumber.ResultNumber;
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix2(
      Settlement settlement,
      ref float meleeEliteTroopRate,
      ref float rangedEliteTroopRate) {
      if (!(settlement.OwnerClan?.Kingdom?.ActivePolicies.Contains(DefaultPolicies.LandGrantsForVeterans) ?? false))
        return;

      meleeEliteTroopRate += 0.1f;
      rangedEliteTroopRate += 0.1f;
    }

  }

}