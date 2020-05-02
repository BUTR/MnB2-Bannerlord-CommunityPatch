using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class AgrarianPatch : PerkPatchBase<AgrarianPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultVillageProductionCalculatorModel).GetMethod(nameof(DefaultVillageProductionCalculatorModel.CalculateDailyFoodProductionAmount), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgrarianPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xB6, 0x97, 0x7F, 0x48, 0x9D, 0x7D, 0x8D, 0x7F,
        0x17, 0x9B, 0x01, 0xB8, 0xC3, 0x6D, 0x87, 0x1B,
        0x9E, 0xDC, 0xF4, 0xDA, 0x8B, 0xC2, 0xFD, 0x5C,
        0xEE, 0x7C, 0x51, 0x93, 0x1D, 0x4E, 0x93, 0x5F
      }
    };

public AgrarianPatch() : base("XNc2NIGL") {}

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null)
        return false;
      if (Perk.PrimaryBonus != 0f)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      Perk.Modify(0.3f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref float __result, Village village) {
      var perk = ActivePatch.Perk;
      if (!(village.Bound?.Town?.Governor?.GetPerkValue(perk) ?? false))
        return;

      __result += (int) (__result * perk.PrimaryBonus);
    }

  }

}