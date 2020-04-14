using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class FoodRationingPatch : PatchBase<FoodRationingPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementFoodModel).GetMethod("CalculateTownFoodChangeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(FoodRationingPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(FoodRationingPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0xF1, 0x92, 0xE2, 0xAF, 0x91, 0x2C, 0xC6, 0xEF,
        0x8C, 0x06, 0x09, 0x46, 0xBE, 0xC9, 0x90, 0x80,
        0x67, 0xEA, 0x20, 0xB9, 0xB1, 0x18, 0x04, 0x43,
        0x34, 0x83, 0x33, 0x8F, 0x9A, 0x92, 0xC2, 0x2D
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "l4UuWHba");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfoPrefix),
        new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(ref StatExplainer tooltipStringBuilder)
      => tooltipStringBuilder ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref float __result, Town town, StatExplainer tooltipStringBuilder) {
      var perk = ActivePatch._perk;
      if (!town.IsUnderSiege)
        return;

      var leader = town.Settlement?.OwnerClan?.Leader;
      if (leader == null || !leader.GetPerkValue(perk))
        return;

      var explainedNumber = new ExplainedNumber(__result, tooltipStringBuilder);
      explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);

      __result = explainedNumber.BaseNumber;
    }

  }

}