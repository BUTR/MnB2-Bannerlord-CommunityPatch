#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityPatch.Patches.Perks.Intelligence.Engineering.Stubs;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class GoodMaterialsPatch : PatchBase<GoodMaterialsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly Type MapSiegeProductionVmType = Type.GetType("SandBox.ViewModelCollection.MapSiege.MapSiegeProductionVM, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

    private static readonly MethodInfo AiTargetMethodInfo = typeof(SiegeEvent).GetMethod(nameof(SiegeEvent.DoSiegeAction), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PlayerTargetMethodInfo = MapSiegeProductionVmType.GetMethod("OnPossibleMachineSelection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo MaxHitPointsTargetMethodInfo = typeof(SiegeEvent.SiegeEngineConstructionProgress).GetMethod("get_MaxHitPoints", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo TooltipTargetMethodInfo = SiegeTooltipHelper.TargetMethodInfo;

    private static readonly MethodInfo AiPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(AiPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PlayerPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(PlayerPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PlayerPatchMethodInfoPrefix = typeof(GoodMaterialsPatch).GetMethod(nameof(PlayerPrefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo MaxHitPointsPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(MaxHitPointsPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo TooltipPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(TooltipPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly PropertyInfo MapSiegeProductionVmSiegeProperty = MapSiegeProductionVmType.GetProperty("Siege", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly PropertyInfo MapSiegeProductionVmPlayerSideProperty = MapSiegeProductionVmType.GetProperty("PlayerSide", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AiTargetMethodInfo;
      yield return PlayerTargetMethodInfo;
      yield return MaxHitPointsTargetMethodInfo;
    }

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "EJCrymMr");

    private PerkObject _perk;

    private static readonly byte[][] AiHashes = {
      new byte[] {
        // e1.1.0.225190
        0x27, 0x47, 0x64, 0x00, 0xA1, 0x01, 0x5C, 0x6B,
        0xDF, 0x91, 0x21, 0x34, 0xD6, 0x97, 0x63, 0x11,
        0x5D, 0xF1, 0x1D, 0xA9, 0x64, 0x6B, 0xB6, 0x11,
        0x2D, 0x7C, 0x3F, 0xF2, 0xE6, 0xF3, 0x70, 0xAE
      }
    };

    private static readonly byte[][] PlayerHashes = {
      new byte[] {
        // e1.1.0.225190
        0x6A, 0x17, 0x74, 0xF3, 0xA5, 0xF0, 0xCC, 0x28,
        0xD4, 0x9D, 0xE8, 0x07, 0xB4, 0x29, 0x5F, 0xA4,
        0x19, 0xB3, 0x35, 0x68, 0xF5, 0xA7, 0xCB, 0xDD,
        0x1D, 0xD0, 0xCA, 0x8C, 0xDE, 0x57, 0xD5, 0xA5
      }
    };

    private static readonly byte[][] MaxHitPointsHashes = {
      new byte[] {
        // e1.1.0.225190
        0xA0, 0x5E, 0x45, 0x04, 0x48, 0xD6, 0xCF, 0xBB,
        0xE9, 0xB0, 0x79, 0xD5, 0x83, 0x3A, 0xEB, 0x95,
        0x6B, 0xF4, 0x6D, 0x60, 0x3D, 0x9A, 0x63, 0x42,
        0x74, 0xCC, 0x14, 0x54, 0x6C, 0xA7, 0xCD, 0x98
      }
    };

    private static readonly byte[][] TooltipHashes = SiegeTooltipHelper.TooltipHashes;

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;
      if (PlayerTargetMethodInfo == null) return false;
      if (MaxHitPointsTargetMethodInfo == null) return false;
      if (TooltipTargetMethodInfo == null) return false;

      var aiPatchInfo = Harmony.GetPatchInfo(AiTargetMethodInfo);
      if (AlreadyPatchedByOthers(aiPatchInfo)) return false;

      var playerPatchInfo = Harmony.GetPatchInfo(PlayerTargetMethodInfo);
      if (AlreadyPatchedByOthers(playerPatchInfo)) return false;

      var maxHitPointsPatchInfo = Harmony.GetPatchInfo(MaxHitPointsTargetMethodInfo);
      if (AlreadyPatchedByOthers(maxHitPointsPatchInfo)) return false;

      var tooltipPatchInfo = Harmony.GetPatchInfo(TooltipTargetMethodInfo);
      if (AlreadyPatchedByOthers(tooltipPatchInfo)) return false;

      var aiHash = AiTargetMethodInfo.MakeCilSignatureSha256();
      var playerHash = PlayerTargetMethodInfo.MakeCilSignatureSha256();
      var maxHitPointsHashes = MaxHitPointsTargetMethodInfo.MakeCilSignatureSha256();
      var tooltipHashes = TooltipTargetMethodInfo.MakeCilSignatureSha256();

      return aiHash.MatchesAnySha256(AiHashes) &&
        playerHash.MatchesAnySha256(PlayerHashes) &&
        maxHitPointsHashes.MatchesAnySha256(MaxHitPointsHashes) &&
        tooltipHashes.MatchesAnySha256(TooltipHashes);
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );

      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 20f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(AiTargetMethodInfo, postfix: new HarmonyMethod(AiPatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(PlayerTargetMethodInfo, postfix: new HarmonyMethod(PlayerPatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(PlayerTargetMethodInfo, new HarmonyMethod(PlayerPatchMethodInfoPrefix));
      CommunityPatchSubModule.Harmony.Patch(MaxHitPointsTargetMethodInfo, postfix: new HarmonyMethod(MaxHitPointsPatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(TooltipTargetMethodInfo, postfix: new HarmonyMethod(TooltipPatchMethodInfoPostfix));

      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AiPostfix(ref SiegeEvent __instance, SiegeEvent.SiegeEnginesContainer siegeEngines, SiegeStrategyActionModel.SiegeAction siegeAction) {
      if (siegeAction != SiegeStrategyActionModel.SiegeAction.ConstructNewSiegeEngine) return;

      var deployedSiegeEngines = siegeEngines.DeployedSiegeEngines;
      var justDeployedEngine = deployedSiegeEngines.LastOrDefault();
      if (justDeployedEngine == null) return;

      var sideSiegeEvent = GetSiegeContainerSide(__instance, siegeEngines);
      ApplyPerkToSiegeEngine(justDeployedEngine, sideSiegeEvent);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    // ReSharper disable once RedundantAssignment
    public static void PlayerPrefix(ref Object __instance, out (int DeployedCount, int ReservedCount) __state) {
      var siegeEvent = GetSiegeEventFromVm(__instance);
      if (siegeEvent == null) {
        __state = (0, 0);
        return;
      }

      var playerSideSiegeEvent = siegeEvent.GetSiegeEventSide(GetPlayerSideFromVm(__instance));
      var deployedSiegeEngineCount = playerSideSiegeEvent.SiegeEngines.DeployedSiegeEngines.Count;
      var reservedSiegeEngineCount = playerSideSiegeEvent.SiegeEngines.ReservedSiegeEngines.Count;
      __state = (deployedSiegeEngineCount, reservedSiegeEngineCount);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void PlayerPostfix(ref Object __instance, ref (int DeployedCount, int ReservedCount) __state) {
      var siegeEvent = GetSiegeEventFromVm(__instance);
      var playerSideSiegeEvent = siegeEvent.GetSiegeEventSide(GetPlayerSideFromVm(__instance));

      if (!HasSiegeEngineJustBeenConstructed(playerSideSiegeEvent, __state.DeployedCount, __state.ReservedCount)) return;

      var justDeployedEngine = playerSideSiegeEvent.SiegeEngines.DeployedSiegeEngines.Last();
      ApplyPerkToSiegeEngine(justDeployedEngine, playerSideSiegeEvent);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TooltipPostfix(ref List<TooltipProperty> __result, SiegeEvent.SiegeEngineConstructionProgress? engineInProgress = null) {
      var siegeEventSide = SiegeTooltipHelper.GetConstructionSiegeEventSide(engineInProgress);
      if (siegeEventSide == null || engineInProgress == null) return;

      CalculateBonusFlatHpAndRateFromPerk(engineInProgress, siegeEventSide, out var bonusFlatHp, out var bonusHpRate);
      SiegeTooltipHelper.AddPerkTooltip(__result, ActivePatch._perk, bonusHpRate);
      SiegeTooltipHelper.UpdateMaxHpTooltip(__result, bonusFlatHp);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    // ReSharper disable once RedundantAssignment
    public static void MaxHitPointsPostfix(ref float __result, ref SiegeEvent.SiegeEngineConstructionProgress __instance)
      => __result = SiegeEngineConstructionExtraDataManager.GetMaxHitPoints(__instance);

    private static ISiegeEventSide GetSiegeContainerSide(SiegeEvent siegeEvent, SiegeEvent.SiegeEnginesContainer siegeEngines)
      => siegeEvent.GetSiegeEventSide(siegeEvent.BesiegerCamp.SiegeEngines == siegeEngines ? BattleSideEnum.Attacker : BattleSideEnum.Defender);

    private static void ApplyPerkToSiegeEngine(SiegeEvent.SiegeEngineConstructionProgress justDeployedEngine, ISiegeEventSide sideSiegeEvent) {
      CalculateBonusFlatHpAndRateFromPerk(justDeployedEngine, sideSiegeEvent, out var bonusFlatHp, out _);
      justDeployedEngine.SetHitpoints(justDeployedEngine.Hitpoints + bonusFlatHp);
      SiegeEngineConstructionExtraDataManager.SetMaxHitPoints(justDeployedEngine, justDeployedEngine.Hitpoints);
    }

    private static SiegeEvent? GetSiegeEventFromVm(object vm)
      => MapSiegeProductionVmSiegeProperty?.GetValue(vm) as SiegeEvent;

    private static BattleSideEnum GetPlayerSideFromVm(object vm)
      => (BattleSideEnum) (MapSiegeProductionVmPlayerSideProperty != null ? MapSiegeProductionVmPlayerSideProperty.GetValue(vm) : BattleSideEnum.None);

    private static bool HasSiegeEngineJustBeenConstructed(ISiegeEventSide playerSiegeEvent, int deployedSiegeEngineCount, int reservedSiegeEngineCount) {
      if (playerSiegeEvent.SiegeEngines.DeployedSiegeEngines.Count <= deployedSiegeEngineCount) return false;

      return playerSiegeEvent.SiegeEngines.ReservedSiegeEngines.Count == reservedSiegeEngineCount;
    }

    private static void CalculateBonusFlatHpAndRateFromPerk(SiegeEvent.SiegeEngineConstructionProgress justDeployedEngine,
      ISiegeEventSide sideSiegeEvent, out float bonusFlatHp, out float bonusHpRate) {
      var perk = ActivePatch._perk;
      var partyMemberHealth = new ExplainedNumber(justDeployedEngine.SiegeEngine.MaxHitPoints);
      var partyMemberRate = new ExplainedNumber(100);

      foreach (var siegeParty in sideSiegeEvent.SiegeParties.Where(x => x.MobileParty != null)) {
        PerkHelper.AddPerkBonusForParty(perk, siegeParty.MobileParty, ref partyMemberHealth);
        PerkHelper.AddPerkBonusForParty(perk, siegeParty.MobileParty, ref partyMemberRate);
      }

      bonusFlatHp = partyMemberHealth.ResultNumber - partyMemberHealth.BaseNumber;
      bonusHpRate = partyMemberRate.ResultNumber - partyMemberRate.BaseNumber;
    }

  }

}