using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  // v1.3 and up, see BuilderPatch2
  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 3)]
  public sealed class BuilderPatch : PerkPatchBase<BuilderPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPower), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo WithoutBoostTargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPowerWithoutBoost), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(BuilderPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return WithoutBoostTargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x51, 0xAD, 0x16, 0x28, 0x7C, 0x93, 0x7A, 0x2E,
        0xBA, 0x26, 0xF9, 0xFD, 0x67, 0x6A, 0x9C, 0xFD,
        0xC9, 0x3F, 0xD4, 0x45, 0x53, 0x66, 0x00, 0x9A,
        0x51, 0x2C, 0x5C, 0x04, 0x23, 0xC8, 0xF4, 0x85
      }
    };

    public static readonly byte[][] WithoutBoostHashes = {
      new byte[] {
        // e1.1.0.225190
        0x8C, 0x3A, 0x1F, 0xC9, 0xAE, 0x69, 0x29, 0x4B,
        0xC7, 0xCB, 0x77, 0x86, 0x9F, 0x0C, 0x1D, 0x0F,
        0x90, 0x72, 0x5D, 0x21, 0xD0, 0xFF, 0xA7, 0x92,
        0x04, 0x9F, 0xDA, 0x73, 0x49, 0x15, 0x11, 0x7D
      }
    };

    public BuilderPatch() : base("dsNV3sgp") {
    }

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (HarmonyHelpers.AlreadyPatchedByOthers(patchInfo)) return false;

      return TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes)
        && WithoutBoostTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(WithoutBoostHashes);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(.5f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(WithoutBoostTargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(ref int __result, Town town, StatExplainer explanation = null)
      => TryToApplyBuilderPerk(ref __result, town, explanation);

    private static void TryToApplyBuilderPerk(ref int productionPower, Town town, StatExplainer explanation = null) {
      if (!HasGovernorWithBuilderPerk(town)) return;

      var perk = ActivePatch.Perk;
      var productionPowerBonus = new ExplainedNumber(productionPower, explanation);
      productionPowerBonus.AddFactor(perk.PrimaryBonus, perk.Name);

      productionPower = (int) productionPowerBonus.ResultNumber;
    }

    private static bool HasGovernorWithBuilderPerk(Town town)
      => town.Governor?.GetPerkValue(ActivePatch.Perk) == true;

  }

}