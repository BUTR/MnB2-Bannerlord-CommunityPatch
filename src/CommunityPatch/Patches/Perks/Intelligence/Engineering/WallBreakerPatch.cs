using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class WallBreakerPatch : PatchBase<WallBreakerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(BesiegerCamp).GetMethod("BombardHitWalls", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo TooltipTargetMethodInfo = SiegeTooltipHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(WallBreakerPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo TooltipPatchMethodInfo = typeof(WallBreakerPatch).GetMethod(nameof(TooltipPostfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return TooltipTargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] TooltipHashes = SiegeTooltipHelper.TooltipHashes;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xE1, 0xB4, 0xCD, 0xF0, 0x16, 0xF2, 0xF1, 0x9F,
        0x3B, 0x2E, 0x0E, 0x73, 0x72, 0x2F, 0xBF, 0x76,
        0x60, 0x1E, 0xDA, 0x2E, 0xAF, 0xF7, 0x46, 0xB9,
        0x5F, 0xE0, 0x48, 0x0B, 0x1E, 0xC5, 0x70, 0x7F
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "0wlWgIeL");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;
      if (TargetMethodInfo == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var tooltipPatchInfo = Harmony.GetPatchInfo(TooltipTargetMethodInfo);
      if (AlreadyPatchedByOthers(tooltipPatchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      var tooltipHash = TooltipTargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes) && tooltipHash.MatchesAnySha256(TooltipHashes);
    }

    public override void Apply(Game game) {
      _perk.SetPrimaryBonus(25f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(TooltipTargetMethodInfo, postfix: new HarmonyMethod(TooltipPatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Prefix(BesiegerCamp __instance, SiegeEngineType attackerEngineType, int wallIndex) {
      var siegeEventSide = __instance.SiegeEvent?.GetSiegeEventSide(BattleSideEnum.Attacker);
      if (siegeEventSide == null) return;

      var besiegedSettlement = __instance.SiegeEvent.BesiegedSettlement;
      CalculateBonusDamageAndRates(attackerEngineType, siegeEventSide, out _, out var bonusDamage);

      var wallSections = besiegedSettlement.SettlementWallSectionHitPointsRatioList;
      if (wallSections[wallIndex] <= 0f) return;

      var targetWallSectionPercentageHp = wallSections[wallIndex];
      var percentageDamageToWallSection = bonusDamage / besiegedSettlement.MaxHitPointsOfOneWallSection;
      var targetWallSectionFinalHp = MBMath.ClampFloat(targetWallSectionPercentageHp - percentageDamageToWallSection, 0f, 1f);
      besiegedSettlement.SetWallSectionHitPointsRatioAtIndex(wallIndex, targetWallSectionFinalHp);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TooltipPostfix(ref List<TooltipProperty> __result, SiegeEvent.SiegeEngineConstructionProgress engineInProgress = null) {
      var siegeEventSide = SiegeTooltipHelper.GetConstructionSiegeEventSide(engineInProgress);
      if (siegeEventSide == null) return;

      CalculateBonusDamageAndRates(engineInProgress.SiegeEngine, siegeEventSide, out var bonusRate, out var bonusDamage);
      SiegeTooltipHelper.AddPerkTooltip(__result, ActivePatch._perk, bonusRate);
      SiegeTooltipHelper.UpdateRangedDamageToWallsTooltip(__result, bonusDamage);
    }

    private static void CalculateBonusDamageAndRates(
      SiegeEngineType siegeEngineType,
      ISiegeEventSide siegeEventSide, out float bonusRateOnly, out float bonusDamageOnly) {
      var perk = ActivePatch._perk;
      var baseDamage = siegeEngineType.Damage;
      var partyMemberDamage = new ExplainedNumber(baseDamage);
      var partyMemberRate = new ExplainedNumber(100f);
      var parties = siegeEventSide.SiegeParties.Where(x => x.MobileParty != null);

      foreach (var party in parties) {
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberRate);
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberDamage);
      }

      bonusRateOnly = partyMemberRate.ResultNumber - 100;
      bonusDamageOnly = partyMemberDamage.ResultNumber - baseDamage;
    }

  }

}