using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class EnhancedMinesPatch : PerkPatchBase<EnhancedMinesPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateVillageTaxFromIncome", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(EnhancedMinesPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x83, 0xCF, 0x66, 0x4A, 0x31, 0x3B, 0xF7, 0x98,
        0xBE, 0xB8, 0x98, 0xA4, 0x85, 0x4D, 0xED, 0xA2,
        0xCE, 0x37, 0xC1, 0x0E, 0x34, 0x2C, 0xB8, 0x84,
        0x0E, 0xB2, 0x61, 0xA8, 0xB5, 0x97, 0x93, 0x08
      }
    };

    public EnhancedMinesPatch() : base("6oE7rB6q") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.Modify(0.5f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once t
    public static void Postfix(ref int __result, Village village, int marketIncome) {
      var perk = ActivePatch.Perk;
      if (!(village.Bound?.OwnerClan?.Leader?.GetPerkValue(perk) ?? false))
        return;

      if (village.VillageType.PrimaryProduction.IsFood)
        return;

      var explainedNumber = new ExplainedNumber(__result);
      explainedNumber.AddFactor(perk.PrimaryBonus, perk.Description);
      __result = (int) explainedNumber.ResultNumber;
    }

  }

}