using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class FoodRationingPatch : PerkPatchBase<FoodRationingPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementFoodModel).GetMethod("CalculateTownFoodChangeInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(FoodRationingPatch).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(FoodRationingPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0xF1, 0x92, 0xE2, 0xAF, 0x91, 0x2C, 0xC6, 0xEF,
        0x8C, 0x06, 0x09, 0x46, 0xBE, 0xC9, 0x90, 0x80,
        0x67, 0xEA, 0x20, 0xB9, 0xB1, 0x18, 0x04, 0x43,
        0x34, 0x83, 0x33, 0x8F, 0x9A, 0x92, 0xC2, 0x2D
      },
      new byte[] {
        // e1.3.0.227640
        0xB5, 0x0F, 0xD0, 0xED, 0xDF, 0x23, 0x30, 0xD2,
        0xB4, 0x85, 0xB0, 0x5B, 0x2D, 0x4D, 0xC7, 0x20,
        0x20, 0x74, 0xCC, 0xA5, 0x57, 0x3C, 0xCB, 0x3A,
        0x8A, 0x74, 0x9C, 0x3B, 0x0C, 0x77, 0x28, 0x86
      }
    };

    public FoodRationingPatch() : base("l4UuWHba") {
    }

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

    private static void Prefix(ref StatExplainer tooltipStringBuilder)
      => tooltipStringBuilder ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref float __result, Town town, StatExplainer tooltipStringBuilder) {
      var perk = ActivePatch.Perk;
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