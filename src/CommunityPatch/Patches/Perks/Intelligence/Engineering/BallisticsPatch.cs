using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using TaleWorlds.Core.ViewModelCollection;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public sealed class BallisticsPatch : PatchBase<BallisticsPatch> {

    public override bool Applied { get; protected set; }
    
    private static readonly MethodInfo TooltipTargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.SandBoxUIHelper, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetSiegeEngineTooltip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo BombardTargetMethodInfo =
      typeof(SiegeEvent).GetMethod("BombardHitEngine", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo TooltipPatchMethodInfo = typeof(BallisticsPatch).GetMethod(nameof(TooltipPostfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo BombardPatchMethodInfo = typeof(BallisticsPatch).GetMethod(nameof(BombardPrefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TooltipTargetMethodInfo;
      yield return BombardTargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] TooltipHashes = {
      new byte[] {
        // e1.1.0.225190
        0x14, 0x86, 0x09, 0xC4, 0xB3, 0xA8, 0xFD, 0xB5,
        0x53, 0x01, 0x37, 0x83, 0x29, 0x96, 0xA4, 0x8F,
        0xCE, 0xD5, 0xA5, 0xC9, 0x29, 0xDF, 0xA0, 0x6E,
        0xD9, 0x5C, 0xD5, 0x37, 0xAC, 0x48, 0x20, 0x64
      }
    };
    
    private static readonly byte[][] BombardHashes = {
      new byte[] {
        // e1.1.0.225190
        0x97, 0xF2, 0xEB, 0x6F, 0xD0, 0x02, 0x95, 0x39,
        0x50, 0xEF, 0x10, 0x9B, 0x78, 0x8C, 0xEF, 0xDC,
        0x42, 0x30, 0x5E, 0x08, 0x02, 0xCE, 0x7E, 0x56,
        0x53, 0x60, 0x27, 0xA9, 0x84, 0x1C, 0xC3, 0xF2
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "LyVZYGkN");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;
      if (TooltipTargetMethodInfo == null) return false;
      if (BombardTargetMethodInfo == null) return false;

      var tooltipPatchInfo = Harmony.GetPatchInfo(TooltipTargetMethodInfo);
      if (AlreadyPatchedByOthers(tooltipPatchInfo)) return false;
      
      var bombardPatchInfo = Harmony.GetPatchInfo(BombardTargetMethodInfo);
      if (AlreadyPatchedByOthers(bombardPatchInfo)) return false;

      var tooltipHash = TooltipTargetMethodInfo.MakeCilSignatureSha256();
      var bombardHash = BombardTargetMethodInfo.MakeCilSignatureSha256();
      return tooltipHash.MatchesAnySha256(TooltipHashes) && bombardHash.MatchesAnySha256(BombardHashes);
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
        SkillEffect.PerkRole.PartyLeader, _perk.PrimaryBonus,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TooltipTargetMethodInfo, postfix: new HarmonyMethod(TooltipPatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(BombardTargetMethodInfo, new HarmonyMethod(BombardPatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TooltipPostfix(ref List<TooltipProperty> __result, SiegeEngineType engine) {
      var perk = ActivePatch._perk;

      if (!IsCatapult(engine)) return;
      if (!DefendersFromHeroSiegeHaveBallisticPerk()) return;

      var emptyTooltip = __result?.LastOrDefault();
      if (emptyTooltip == null) return;
      emptyTooltip.DefinitionLabel = perk.Name.ToString();
      emptyTooltip.ValueLabel = $"{(perk.PrimaryBonus * 100):F1}%";

      var rangedAttackProperty = FindRangedAttackTooltipProperty(__result);
      if (rangedAttackProperty == null) return;
      var rangedDamage = (int)Convert.ToDouble(rangedAttackProperty.ValueLabel);
      var totalDamage = rangedDamage + rangedDamage * perk.PrimaryBonus;
      rangedAttackProperty.ValueLabel = $"{totalDamage:F1}";
    }
    
    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void BombardPrefix(ISiegeEventSide siegeEventSide, SiegeEngineType attackerEngineType, SiegeEvent.SiegeEngineConstructionProgress damagedEngine) {
      var perk = ActivePatch._perk;

      if (!IsCatapult(attackerEngineType)) return;
      if (!PartiesHaveBallisticPerk(siegeEventSide.SiegeParties)) return;
      
      var bonusDamage = attackerEngineType.Damage * perk.PrimaryBonus;
      damagedEngine.SetHitpoints(damagedEngine.Hitpoints - bonusDamage);
    }

    private static bool IsCatapult(SiegeEngineType engine)
      => engine == DefaultSiegeEngineTypes.Catapult || engine == DefaultSiegeEngineTypes.FireCatapult;
    
    private static bool DefendersFromHeroSiegeHaveBallisticPerk() {
      if (Hero.MainHero == null) return false;

      var settlement = Hero.MainHero.CurrentSettlement ?? Hero.MainHero.PartyBelongedTo.BesiegedSettlement;
      var siegeEvent = settlement.SiegeEvent;
      var defenderSiegeEvent = siegeEvent.GetSiegeEventSide(BattleSideEnum.Defender);
      var defenders = defenderSiegeEvent.SiegeParties;

      return PartiesHaveBallisticPerk(defenders);
    }

    private static bool PartiesHaveBallisticPerk(IEnumerable<PartyBase> defenders)
      => defenders.Any(x => x.LeaderHero?.GetPerkValue(ActivePatch._perk) == true);

    private static TooltipProperty FindRangedAttackTooltipProperty(List<TooltipProperty> properties) 
      =>  properties.FirstOrDefault(x => 
        x.DefinitionLabel == GameTexts.FindText("str_projectile_damage").ToString());
  }
}