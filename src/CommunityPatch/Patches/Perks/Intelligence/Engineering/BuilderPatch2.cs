using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 3)]
  public sealed class BuilderPatch2 : PerkPatchBase<BuilderPatch2> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPower), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo WithoutBoostTargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPowerWithoutBoost), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo InternalTargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod("CalculateDailyConstructionPowerInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(BuilderPatch2).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return WithoutBoostTargetMethodInfo;
      yield return InternalTargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.227640
        0x8C, 0xA5, 0x03, 0xCD, 0x24, 0xED, 0x16, 0x09,
        0xE6, 0x1E, 0xFF, 0x13, 0x7B, 0x3E, 0x3D, 0x89,
        0x1F, 0x6C, 0xD6, 0x38, 0xE3, 0xED, 0x2D, 0xB5,
        0xD2, 0x35, 0xE7, 0x0B, 0x55, 0x4F, 0x5C, 0xDD
      }
    };

    public static readonly byte[][] WithoutBoostHashes = {
      new byte[] {
        // e1.3.0.227640
        0x0F, 0xF5, 0x05, 0xA4, 0x0C, 0xD8, 0xFF, 0x1E,
        0x00, 0xBC, 0xB9, 0x5F, 0x75, 0x19, 0xD9, 0x83,
        0x61, 0x86, 0x15, 0x0F, 0xF0, 0x8E, 0xC0, 0x79,
        0x08, 0x4C, 0x13, 0x15, 0x0B, 0xC4, 0x17, 0x85
      }
    };

    public static readonly byte[][] InternalHashes = {
      new byte[] {
        // e1.3.0.227640
        0xA8, 0x9F, 0x05, 0x12, 0xD8, 0x80, 0xD4, 0x55,
        0xEE, 0xF2, 0x49, 0xC8, 0xF5, 0x60, 0x2A, 0xE1,
        0x0D, 0x8A, 0xF1, 0x4D, 0xC3, 0x69, 0xF2, 0x8F,
        0x91, 0x06, 0xB9, 0xF2, 0xBA, 0xC7, 0xCC, 0x08
      },
      new byte[] {
        // e1.4.0.228531
        0xA4, 0xA5, 0xE2, 0x69, 0xE8, 0xE6, 0xD1, 0x9E,
        0x5C, 0x74, 0x45, 0xAB, 0x04, 0x24, 0x17, 0x9F,
        0xB8, 0xEA, 0x2B, 0x8B, 0x0D, 0xF1, 0x42, 0xFA,
        0x93, 0x96, 0x6E, 0x2F, 0xE8, 0x1D, 0xD2, 0xA9
      },
      new byte[] {
        // e1.4.1.231234
        0x3D, 0xE0, 0xD1, 0x8C, 0x51, 0xB3, 0x12, 0xF7,
        0x0C, 0xB7, 0x01, 0x82, 0xD6, 0x5D, 0x93, 0x21,
        0x1C, 0x19, 0xF3, 0x6E, 0x1C, 0x6D, 0x9E, 0x97,
        0xB7, 0x2B, 0xA1, 0xA0, 0x08, 0xBC, 0x3B, 0x8E
      }
    };

    public BuilderPatch2() : base("dsNV3sgp") {
    }

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      if (!TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes)
        || !WithoutBoostTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(WithoutBoostHashes)
        || !InternalTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(InternalHashes))
        return false;

      return base.IsApplicable(game);
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