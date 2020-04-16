using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class ConstructionExpertPatch : PatchBase<ConstructionExpertPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(Building).GetMethod(nameof(Building.GetConstructionCost), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(ConstructionExpertPatch).GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x8D, 0x47, 0x93, 0x8C, 0xC0, 0x9B, 0x38, 0xB9,
        0xB5, 0x6C, 0xA7, 0x97, 0x99, 0x0D, 0xCB, 0x2B,
        0x93, 0x01, 0x8B, 0x65, 0xE6, 0x0C, 0x1F, 0x21,
        0x0D, 0x00, 0x03, 0xF4, 0x4E, 0xDB, 0x2B, 0x15
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "KBcNYbIC");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );
      // most of the properties of skills have private setters, yet Initialize is public
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, _perk.PrimaryBonus,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfoPrefix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Prefix(ref int __result, Building __instance) {
      var perk = ActivePatch._perk;
      var productionCostRate = 1f;
      
      if (IsCastleChartersPolicyActive(__instance)) productionCostRate -= 0.2f;
      if (ShouldApplyConstructionExpertPerk(__instance)) productionCostRate -= perk.PrimaryBonus;
      
      var productionCost = __instance.BuildingType.GetProductionCost(__instance.CurrentLevel) * productionCostRate;

      __result += (int) productionCost;

      return false;
    }

    private static bool IsCastleChartersPolicyActive(Building building)
      => building.Town.Settlement.OwnerClan.Kingdom != null &&
        building.Town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CastleCharters);

    private static bool ShouldApplyConstructionExpertPerk(Building building)
      => HasGovernorWithConstructionExpert(building) && IsWallOrFortification(building);
    
    private static bool HasGovernorWithConstructionExpert(Building building)
      => building.Town.Governor?.GetPerkValue(ActivePatch._perk) == true;

    private static bool IsWallOrFortification(Building building)
      => building.BuildingType == DefaultBuildingTypes.Wall || building.BuildingType == DefaultBuildingTypes.Fortifications;
  }

}