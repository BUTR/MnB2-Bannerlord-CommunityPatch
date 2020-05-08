using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  // appears to be fixed in 1.3?
  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 3)]
  public sealed class ConstructionExpertPatch : PerkPatchBase<ConstructionExpertPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPower), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo WithoutBoostTargetMethodInfo =
      typeof(DefaultBuildingConstructionModel).GetMethod(nameof(DefaultBuildingConstructionModel.CalculateDailyConstructionPowerWithoutBoost), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ConstructionExpertPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

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

    public ConstructionExpertPatch() : base("KBcNYbIC") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      var withoutBoostHash = WithoutBoostTargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes) && withoutBoostHash.MatchesAnySha256(WithoutBoostHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(WithoutBoostTargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(ref int __result, Town town, StatExplainer explanation = null)
      => TryToApplyConstructionExpert(ref __result, town, explanation);

    private static void TryToApplyConstructionExpert(ref int productionPower, Town town, StatExplainer explanation = null) {
      var perk = ActivePatch.Perk;

      if (town.BuildingsInProgress.IsEmpty()) return;

      var building = town.BuildingsInProgress.Peek();

      if (!ShouldApplyConstructionExpertPerk(town, building)) return;

      var productionPowerBonus = new ExplainedNumber(productionPower, explanation);
      productionPowerBonus.AddFactor(perk.PrimaryBonus, perk.Name);

      productionPower = (int) productionPowerBonus.ResultNumber;
    }

    private static bool ShouldApplyConstructionExpertPerk(Town town, Building building)
      => HasGovernorWithConstructionExpert(town) && IsWallOrFortificationBuilding(building);

    private static bool HasGovernorWithConstructionExpert(Town town)
      => town.Governor?.GetPerkValue(ActivePatch.Perk) == true;

    private static bool IsWallOrFortificationBuilding(Building building)
      => building.BuildingType == DefaultBuildingTypes.Wall || building.BuildingType == DefaultBuildingTypes.Fortifications;

  }

}