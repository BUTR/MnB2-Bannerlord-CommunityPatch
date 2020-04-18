using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public static class SiegeTooltipHelper {
    public static readonly MethodInfo TargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.SandBoxUIHelper, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetSiegeEngineInProgressTooltip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public static readonly byte[][] TooltipHashes = {
      new byte[] {
        // e1.1.0.225190
        0x68, 0xEE, 0xE7, 0xAD, 0xA8, 0x04, 0xCC, 0x5C,
        0x5E, 0xBA, 0xB6, 0x3D, 0x93, 0xE0, 0x91, 0xC9,
        0x32, 0x6F, 0x65, 0x2E, 0xAF, 0xBF, 0xF3, 0xBD,
        0x51, 0x75, 0x34, 0x1A, 0xDC, 0x0D, 0xD3, 0x25
      }
    };
    
    public static ISiegeEventSide GetConstructionSiegeEventSide(SiegeEvent.SiegeEngineConstructionProgress construction) {
      if (construction?.SiegeEngine == null) return null;
      
      var siegeEvent = GetHeroSiegeEvent();
      if (siegeEvent == null) return null;

      var attackerSide = siegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker);
      if (attackerSide.SiegeEngines.DeployedSiegeEngines.Contains(construction)) return attackerSide;
      
      var defenderSide = siegeEvent.GetSiegeEventSide(BattleSideEnum.Defender);
      return defenderSide;
    }
    
    public static void UpdateRangedDamageTooltip(List<TooltipProperty> tooltips, float bonusDamageOnly)
    {
      var rangedDamageProperty = FindRangedDamageTooltipProperty(tooltips);
      if (rangedDamageProperty == null) return;

      Double.TryParse(rangedDamageProperty.ValueLabel, out var currentRangedDamage);
      var amplifiedRangedDamage = (int) (currentRangedDamage + bonusDamageOnly);
      rangedDamageProperty.ValueLabel = amplifiedRangedDamage.ToString();
    }
    
    public static void AddPerkTooltip(List<TooltipProperty> tooltips, PerkObject perk, float bonusValue) {
      if (Math.Abs(bonusValue) < 0.05) return;
      var isRate = perk.IncrementType == SkillEffect.EffectIncrementType.AddFactor;
      var suffix = isRate ? "%" : "";
      var tooltip = new TooltipProperty(perk.Name.ToString(), value: $"{bonusValue:F1}{suffix}", 0);
      tooltips.Add(tooltip);
    }
    
    private static SiegeEvent GetHeroSiegeEvent() {
      if (Hero.MainHero == null) return null;
      var heroParty = Hero.MainHero.PartyBelongedTo;
      
      return heroParty.BesiegedSettlement?.SiegeEvent ?? 
        heroParty.BesiegerCamp?.SiegeEvent ?? 
        heroParty.CurrentSettlement.SiegeEvent;
    }
    
    private static TooltipProperty FindRangedDamageTooltipProperty(List<TooltipProperty> tooltips) 
      =>  tooltips.FirstOrDefault(x => 
        x.DefinitionLabel == GameTexts.FindText("str_projectile_damage").ToString());
  }

}