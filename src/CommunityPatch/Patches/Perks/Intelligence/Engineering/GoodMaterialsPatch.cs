using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {
  public class GoodMaterialsPatch : PatchBase<GoodMaterialsPatch> {
    public override bool Applied { get; protected set; }

    private static readonly Type MapSiegeProductionVmType = Type.GetType("SandBox.ViewModelCollection.MapSiege.MapSiegeProductionVM, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    
    private static readonly MethodInfo AiTargetMethodInfo =
      typeof(SiegeEvent).GetMethod(nameof(SiegeEvent.DoSiegeAction), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo PlayerTargetMethodInfo = 
      MapSiegeProductionVmType?.GetMethod("OnPossibleMachineSelection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo AiPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(AiPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PlayerPatchMethodInfoPostfix = typeof(GoodMaterialsPatch).GetMethod(nameof(PlayerPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PlayerPatchMethodInfoPrefix = typeof(GoodMaterialsPatch).GetMethod(nameof(PlayerPrefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AiTargetMethodInfo;
      yield return PlayerTargetMethodInfo;
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
    
    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;
      if (PlayerTargetMethodInfo == null) return false;
      
      var aiPatchInfo = Harmony.GetPatchInfo(AiTargetMethodInfo);
      if (AlreadyPatchedByOthers(aiPatchInfo)) return false;
      
      var playerPatchInfo = Harmony.GetPatchInfo(PlayerTargetMethodInfo);
      if (AlreadyPatchedByOthers(playerPatchInfo)) return false;

      var aiHash = AiTargetMethodInfo.MakeCilSignatureSha256();
      var playerHash = PlayerTargetMethodInfo.MakeCilSignatureSha256();
      return aiHash.MatchesAnySha256(AiHashes) && playerHash.MatchesAnySha256(PlayerHashes);
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

      Applied = true;
    }
    
    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AiPostfix(ref SiegeEvent __instance, SiegeEvent.SiegeEnginesContainer siegeEngines, SiegeStrategyActionModel.SiegeAction siegeAction, SiegeEngineType siegeEngineType, int deploymentIndex, int reserveIndex) {
      if (siegeAction != SiegeStrategyActionModel.SiegeAction.ConstructNewSiegeEngine) return;

      var deployedSiegeEngines = siegeEngines.DeployedSiegeEngines;
      var justDeployedEngine = deployedSiegeEngines.LastOrDefault();
      if (justDeployedEngine == null) return;

      var sideSiegeEvent = GetSiegeContainerSide(__instance, siegeEngines);
      ApplyPerkToSiegeEngine(justDeployedEngine, sideSiegeEvent);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    // ReSharper disable once RedundantAssignment
    public static void PlayerPrefix(ref Object __instance, ref Tuple<int, int> __state) {
      var siegeEvent = GetSiegeEventFromVm(__instance);
      var playerSideSiegeEvent = siegeEvent.GetSiegeEventSide(GetPlayerSideFromVm(__instance));
      var deployedSiegeEngineCount = playerSideSiegeEvent.SiegeEngines.DeployedSiegeEngines.Count;
      var reservedSiegeEngineCount = playerSideSiegeEvent.SiegeEngines.ReservedSiegeEngines.Count;
      __state = new Tuple<int, int>(deployedSiegeEngineCount, reservedSiegeEngineCount);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void PlayerPostfix(ref Object __instance, ref Tuple<int, int> __state) {
      var siegeEvent = GetSiegeEventFromVm(__instance);
      var playerSideSiegeEvent = siegeEvent.GetSiegeEventSide(GetPlayerSideFromVm(__instance));
      
      if (!HasSiegeEngineJustBeenConstructed(playerSideSiegeEvent, __state.Item1, __state.Item2)) return;

      var justDeployedEngine = playerSideSiegeEvent.SiegeEngines.DeployedSiegeEngines.Last();
      ApplyPerkToSiegeEngine(justDeployedEngine, playerSideSiegeEvent);
    }

    private static ISiegeEventSide GetSiegeContainerSide(SiegeEvent siegeEvent, SiegeEvent.SiegeEnginesContainer siegeEngines) 
      => siegeEvent.GetSiegeEventSide(siegeEvent.BesiegerCamp.SiegeEngines == siegeEngines ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
    
    private static void ApplyPerkToSiegeEngine(SiegeEvent.SiegeEngineConstructionProgress justDeployedEngine, ISiegeEventSide sideSiegeEvent)
    {
      var perk = ActivePatch._perk;
      var engineHealth = new ExplainedNumber(justDeployedEngine.Hitpoints);

      foreach (var siegeParty in sideSiegeEvent.SiegeParties.Where(x => x.MobileParty != null))
        PerkHelper.AddPerkBonusForParty(perk, siegeParty.MobileParty, ref engineHealth);

      justDeployedEngine.SetHitpoints(engineHealth.ResultNumber);
    }
    
    private static SiegeEvent GetSiegeEventFromVm(object vm) {
      var propertyInfo = MapSiegeProductionVmType.GetProperty("Siege", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      return (SiegeEvent)propertyInfo?.GetValue(vm);
    }
    
    private static BattleSideEnum GetPlayerSideFromVm(object vm) {
      var propertyInfo = MapSiegeProductionVmType.GetProperty("PlayerSide", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      return (BattleSideEnum) (propertyInfo != null ? propertyInfo.GetValue(vm) : BattleSideEnum.None);
    }

    private static bool HasSiegeEngineJustBeenConstructed(ISiegeEventSide playerSiegeEvent, int deployedSiegeEngineCount, int reservedSiegeEngineCount) {
      if (playerSiegeEvent.SiegeEngines.DeployedSiegeEngines.Count <= deployedSiegeEngineCount) return false;
      return playerSiegeEvent.SiegeEngines.ReservedSiegeEngines.Count == reservedSiegeEngineCount;
    }
  }
}