using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class StewardTaxCollectorPatch : PatchBase<StewardTaxCollectorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardTaxCollectorPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        0x0A, 0xCC, 0xF6, 0x94, 0xF3, 0x0D, 0x5E, 0xC9,
        0xDC, 0x61, 0x7D, 0xAE, 0x96, 0xFC, 0x97, 0x80,
        0x11, 0xDC, 0xF9, 0x65, 0x72, 0x3B, 0x82, 0x23,
        0xC2, 0xC7, 0x15, 0x8B, 0x58, 0x35, 0x03, 0x85
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "UbxBYp60");

    public override bool IsApplicable(Game game)
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