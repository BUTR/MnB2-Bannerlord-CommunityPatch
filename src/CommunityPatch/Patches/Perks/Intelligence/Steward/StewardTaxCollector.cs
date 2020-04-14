using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class TaxCollectorPatch : PatchBase<TaxCollectorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(TaxCollectorPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x52, 0xFC, 0xCB, 0x7C, 0xF9, 0x90, 0xBB, 0xAD,
        0xDE, 0xDB, 0x69, 0xE9, 0x9A, 0x41, 0x28, 0x04,
        0x7C, 0xE9, 0xDE, 0xBB, 0xD5, 0xD0, 0xCA, 0x3B,
        0x4B, 0xE3, 0x19, 0x42, 0x1E, 0x36, 0xC8, 0xB2
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "UbxBYp60");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
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