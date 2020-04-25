using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Policies {

  public sealed class LandGrantsForVeteransPatch : PatchBase<LandGrantsForVeteransPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo1 =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo TargetMethodInfo2 =
      typeof(DefaultSettlementMilitiaModel).GetMethod("CalculateMilitiaSpawnRate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1Prefix =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Prefix1), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1Postfix =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Postfix1), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 =
      typeof(LandGrantsForVeteransPatch).GetMethod(nameof(Postfix2), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo1;
      yield return TargetMethodInfo2;
    }

    private static readonly byte[][] Hashes1 = {
      new byte[] {
        // CalculateDailyTaxInternal e1.0.10
        0x52, 0xFC, 0xCB, 0x7C, 0xF9, 0x90, 0xBB, 0xAD,
        0xDE, 0xDB, 0x69, 0xE9, 0x9A, 0x41, 0x28, 0x04,
        0x7C, 0xE9, 0xDE, 0xBB, 0xD5, 0xD0, 0xCA, 0x3B,
        0x4B, 0xE3, 0x19, 0x42, 0x1E, 0x36, 0xC8, 0xB2
      }
    };

    private static readonly byte[][] Hashes2 = {
      new byte[] {
        // CalculateMilitiaSpawnRate e1.0.10
        0xE4, 0x38, 0xF5, 0x91, 0x73, 0x90, 0x52, 0x1E,
        0xCF, 0xC5, 0xE4, 0xCE, 0x3B, 0xA6, 0x3E, 0xFD,
        0x41, 0x26, 0x50, 0xDD, 0x95, 0xC8, 0xC0, 0x86,
        0x81, 0x4C, 0x02, 0xC8, 0xA9, 0xDA, 0xCA, 0xBE
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
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix1(Town town, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix1(ref int __result, Town town, StatExplainer explanation) {
      var kingdom = town.Owner?.Settlement?.OwnerClan?.Kingdom;
      if (kingdom == null)
        return;

      if (kingdom.ActivePolicies.Contains(DefaultPolicies.LandGrantsForVeterans)) {
        var explainedNumber = new ExplainedNumber(__result, explanation);
        if (explainedNumber.BaseNumber > 0) {
          explainedNumber.AddFactor(-0.05f, DefaultPolicies.LandGrantsForVeterans.Name);
          __result = (int) explainedNumber.ResultNumber;
        }
      }
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
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