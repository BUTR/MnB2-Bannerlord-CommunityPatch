using System.Collections.Generic;
using System.Reflection;
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
      },
      new byte[] {
        // e1.4.0.228531
        0xE6, 0x17, 0xD9, 0xA4, 0xCB, 0x62, 0xEE, 0x3A,
        0x72, 0x19, 0x64, 0x61, 0x81, 0x03, 0x27, 0xFE,
        0xE2, 0x7D, 0x54, 0xDC, 0x89, 0x13, 0xDB, 0x3B,
        0x0D, 0x0C, 0x36, 0xE3, 0xED, 0xEC, 0x0D, 0xC7
      },
      new byte[] {
        // e1.4.1.230527
        0xAA, 0x81, 0xC7, 0x57, 0x27, 0x61, 0x68, 0xE3,
        0xF0, 0x93, 0x7A, 0x31, 0x51, 0x2E, 0x43, 0xBD,
        0x16, 0x60, 0xD4, 0xBB, 0xA6, 0x81, 0xE2, 0xBE,
        0xDD, 0xB6, 0x7D, 0xA3, 0x2B, 0xA3, 0x7A, 0xFA
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
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
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