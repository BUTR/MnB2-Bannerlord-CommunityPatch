using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using Helpers;
using TaleWorlds.Core.ViewModelCollection;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class HeavierSiegeEnginesPatch : PatchBase<HeavierSiegeEnginesPatch> {

    public override bool Applied { get; protected set; }
    
    private static readonly MethodInfo TargetMethodInfo =
      typeof(SiegeEvent).GetMethod("BombardHitEngine", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo TooltipTargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.SandBoxUIHelper, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetSiegeEngineInProgressTooltip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(HeavierSiegeEnginesPatch).GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo TooltipPatchMethodInfo = typeof(HeavierSiegeEnginesPatch).GetMethod(nameof(TooltipPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return TooltipTargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x97, 0xF2, 0xEB, 0x6F, 0xD0, 0x02, 0x95, 0x39,
        0x50, 0xEF, 0x10, 0x9B, 0x78, 0x8C, 0xEF, 0xDC,
        0x42, 0x30, 0x5E, 0x08, 0x02, 0xCE, 0x7E, 0x56,
        0x53, 0x60, 0x27, 0xA9, 0x84, 0x1C, 0xC3, 0xF2
      }
    };
    
    private static readonly byte[][] TooltipHashes = {
      new byte[] {
        // e1.1.0.225190
        0x68, 0xEE, 0xE7, 0xAD, 0xA8, 0x04, 0xCC, 0x5C,
        0x5E, 0xBA, 0xB6, 0x3D, 0x93, 0xE0, 0x91, 0xC9,
        0x32, 0x6F, 0x65, 0x2E, 0xAF, 0xBF, 0xF3, 0xBD,
        0x51, 0x75, 0x34, 0x1A, 0xDC, 0x0D, 0xD3, 0x25
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "qXkWSgwA");

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
        _perk.PrimaryRole, 20f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(TooltipTargetMethodInfo, postfix: new HarmonyMethod(TooltipPatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Prefix(ISiegeEventSide siegeEventSide, SiegeEvent.SiegeEngineConstructionProgress damagedEngine) {
      CalculateBonusDamageAndRates(damagedEngine, siegeEventSide, out _, out var bonusDamageOnly);
      damagedEngine.SetHitpoints(damagedEngine.Hitpoints - bonusDamageOnly);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TooltipPostfix(ref List<TooltipProperty> __result, SiegeEvent.SiegeEngineConstructionProgress engineInProgress = null) {
      if (engineInProgress?.SiegeEngine == null) return;
      var siegeEventSide = GetConstructionSiegeEventSide(engineInProgress);
      if (siegeEventSide == null) return;
      
      CalculateBonusDamageAndRates(engineInProgress, siegeEventSide, out var bonusRateOnly, out var bonusDamageOnly);
      AddPerkTooltip(__result, bonusRateOnly);
      UpdateRangedDamageTooltip(__result, bonusDamageOnly);
    }

    private static void CalculateBonusDamageAndRates(
      SiegeEvent.SiegeEngineConstructionProgress engineInProgress,
      ISiegeEventSide siegeEventSide, out float bonusRateOnly, out float bonusDamageOnly) 
    {
      var perk = ActivePatch._perk;
      var baseDamage = engineInProgress.SiegeEngine.Damage;
      var partyMemberDamage = new ExplainedNumber(baseDamage);
      var partyMemberRate = new ExplainedNumber(100f);
      var parties = siegeEventSide.SiegeParties.Where(x => x.MobileParty != null);

      foreach (var party in parties)
      {
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberRate);
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberDamage);
      }

      bonusRateOnly = partyMemberRate.ResultNumber - 100;
      bonusDamageOnly = partyMemberDamage.ResultNumber - baseDamage;
    }

    private static void AddPerkTooltip(List<TooltipProperty> __result, float bonusRateOnly) {
      var perk = ActivePatch._perk;
      var tooltip = new TooltipProperty(perk.Name.ToString(), value: $"{bonusRateOnly:F1}%", 0);
      __result.Add(tooltip);
    }

    private static SiegeEvent GetSiegeEvent() {
      if (Hero.MainHero == null) return null;
      var heroParty = Hero.MainHero.PartyBelongedTo;
      return heroParty.BesiegedSettlement?.SiegeEvent ?? heroParty.BesiegerCamp.SiegeEvent;
    }

    private static ISiegeEventSide GetConstructionSiegeEventSide(SiegeEvent.SiegeEngineConstructionProgress construction) {
      var siegeEvent = GetSiegeEvent();
      if (siegeEvent == null) return null;

      var attackerSide = siegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker);
      if (attackerSide.SiegeEngines.DeployedSiegeEngines.Contains(construction)) return attackerSide;
      
      var defenderSide = siegeEvent.GetSiegeEventSide(BattleSideEnum.Defender);
      return defenderSide;
    }
    
    private static void UpdateRangedDamageTooltip(List<TooltipProperty> __result, float bonusDamageOnly)
    {
      var rangedDamageProperty = FindRangedDamageTooltipProperty(__result);
      if (rangedDamageProperty == null) return;

      Double.TryParse(rangedDamageProperty.ValueLabel, out var currentRangedDamage);
      var amplifiedRangedDamage = (int) (currentRangedDamage + bonusDamageOnly);
      rangedDamageProperty.ValueLabel = amplifiedRangedDamage.ToString();
    }
    
    private static TooltipProperty FindRangedDamageTooltipProperty(List<TooltipProperty> properties) 
      =>  properties.FirstOrDefault(x => 
        x.DefinitionLabel == GameTexts.FindText("str_projectile_damage").ToString());
  }
}