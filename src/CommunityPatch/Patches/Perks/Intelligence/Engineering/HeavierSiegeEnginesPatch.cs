using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using Helpers;
using TaleWorlds.Core.ViewModelCollection;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class HeavierSiegeEnginesPatch : PerkPatchBase<HeavierSiegeEnginesPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(SiegeEvent).GetMethod("BombardHitEngine", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo TooltipTargetMethodInfo = SiegeTooltipHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(HeavierSiegeEnginesPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo TooltipPatchMethodInfo = typeof(HeavierSiegeEnginesPatch).GetMethod(nameof(TooltipPostfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return TooltipTargetMethodInfo;
    }

    public static byte[][] TooltipHashes => SiegeTooltipHelper.TooltipHashes;

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x97, 0xF2, 0xEB, 0x6F, 0xD0, 0x02, 0x95, 0x39,
        0x50, 0xEF, 0x10, 0x9B, 0x78, 0x8C, 0xEF, 0xDC,
        0x42, 0x30, 0x5E, 0x08, 0x02, 0xCE, 0x7E, 0x56,
        0x53, 0x60, 0x27, 0xA9, 0x84, 0x1C, 0xC3, 0xF2
      }
    };

    public HeavierSiegeEnginesPatch() : base("qXkWSgwA") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;
      if (TargetMethodInfo == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var tooltipPatchInfo = Harmony.GetPatchInfo(TooltipTargetMethodInfo);
      if (AlreadyPatchedByOthers(tooltipPatchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      var tooltipHash = TooltipTargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes) || !tooltipHash.MatchesAnySha256(TooltipHashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(20f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(TooltipTargetMethodInfo, postfix: new HarmonyMethod(TooltipPatchMethodInfo));
      Applied = true;
    }

    public static void Prefix(ISiegeEventSide siegeEventSide, SiegeEngineType attackerEngineType, SiegeEvent.SiegeEngineConstructionProgress damagedEngine) {
      CalculateBonusDamageAndRates(attackerEngineType, siegeEventSide, out _, out var bonusDamageOnly);
      damagedEngine.SetHitpoints(damagedEngine.Hitpoints - bonusDamageOnly);
    }

    public static void TooltipPostfix(ref List<TooltipProperty> __result, SiegeEvent.SiegeEngineConstructionProgress engineInProgress = null) {
      var siegeEventSide = SiegeTooltipHelper.GetConstructionSiegeEventSide(engineInProgress);
      if (siegeEventSide == null) return;

      CalculateBonusDamageAndRates(engineInProgress.SiegeEngine, siegeEventSide, out var bonusRateOnly, out var bonusDamageOnly);
      SiegeTooltipHelper.AddPerkTooltip(__result, ActivePatch.Perk, bonusRateOnly);
      SiegeTooltipHelper.UpdateRangedDamageToWallsTooltip(__result, 0);
      SiegeTooltipHelper.UpdateRangedEngineDamageTooltip(__result, bonusDamageOnly);
    }

    private static void CalculateBonusDamageAndRates(
      SiegeEngineType siegeEngineType,
      ISiegeEventSide siegeEventSide, out float bonusRateOnly, out float bonusDamageOnly) {
      var perk = ActivePatch.Perk;
      var baseDamage = siegeEngineType.Damage;
      var partyMemberDamage = new ExplainedNumber(baseDamage);
      var partyMemberRate = new ExplainedNumber(100f);
      var parties = siegeEventSide.SiegeParties.Where(x => x.MobileParty != null);

      foreach (var party in parties) {
#if AFTER_E1_5_1
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, true, ref partyMemberRate);
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, true, ref partyMemberDamage);
#else
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberRate);
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberDamage);
#endif  
      }

      bonusRateOnly = partyMemberRate.ResultNumber - 100;
      bonusDamageOnly = partyMemberDamage.ResultNumber - baseDamage;
    }

  }

}