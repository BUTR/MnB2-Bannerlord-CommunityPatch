using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public static class SiegeTooltipHelper {

    private const string WallDamageLabel = "Projectile Wall Damage";

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

    public static void UpdateRangedEngineDamageTooltip(List<TooltipProperty> tooltips, float bonusDamageOnly) {
      var rangedDamageProperty = FindRangedDamageTooltipProperty(tooltips);
      if (rangedDamageProperty == null) return;

      Double.TryParse(rangedDamageProperty.ValueLabel, out var currentRangedDamage);
      var amplifiedRangedDamage = (int) (currentRangedDamage + bonusDamageOnly);
      rangedDamageProperty.ValueLabel = amplifiedRangedDamage.ToString();
    }

    public static void UpdateMaxHpTooltip(List<TooltipProperty> tooltips, float bonusHp) {
      var property = FindMaxHpTooltipProperty(tooltips);
      if (property == null) return;

      Double.TryParse(property.ValueLabel, out var currentHp);
      var buffedHp = (int) (currentHp + bonusHp);
      property.ValueLabel = buffedHp.ToString();
    }

    public static void AddPerkTooltip(List<TooltipProperty> tooltips, PerkObject perk, float bonusValue) {
      if (bonusValue.IsZero()) return;

      var isRate = perk.IncrementType == SkillEffect.EffectIncrementType.AddFactor;
      var suffix = isRate ? "%" : "";
      var tooltip = new TooltipProperty(perk.Name.ToString(), $"{bonusValue:F1}{suffix}", 0);
      tooltips.Add(tooltip);
    }

    private static SiegeEvent GetHeroSiegeEvent() {
      if (Hero.MainHero == null) return null;

      var heroParty = Hero.MainHero.PartyBelongedTo;

      return heroParty.BesiegedSettlement?.SiegeEvent ??
        heroParty.BesiegerCamp?.SiegeEvent ??
        heroParty.CurrentSettlement.SiegeEvent;
    }

    public static void UpdateRangedDamageToWallsTooltip(List<TooltipProperty> tooltips, float bonusDamage) {
      var wallDamageProperty = FindProjectileWallDamageTooltipProperty(tooltips);

      if (wallDamageProperty == null) {
        var rangedDamageTooltip = FindRangedDamageTooltipProperty(tooltips);
        if (rangedDamageTooltip == null) return;

        wallDamageProperty = new TooltipProperty(WallDamageLabel, rangedDamageTooltip.ValueLabel, 0);
        tooltips.Insert(tooltips.IndexOf(rangedDamageTooltip) + 1, wallDamageProperty);
      }

      Double.TryParse(wallDamageProperty.ValueLabel, out var currentWallDamage);
      var amplifiedWallDamage = (int) (currentWallDamage + bonusDamage);
      wallDamageProperty.ValueLabel = amplifiedWallDamage.ToString();
    }

    private static TooltipProperty FindRangedDamageTooltipProperty(List<TooltipProperty> tooltips)
      => tooltips.FirstOrDefault(x =>
        x.DefinitionLabel == GameTexts.FindText("str_projectile_damage").ToString());

    private static TooltipProperty FindProjectileWallDamageTooltipProperty(List<TooltipProperty> tooltips)
      => tooltips.FirstOrDefault(x => x.DefinitionLabel == WallDamageLabel);

    private static TooltipProperty FindMaxHpTooltipProperty(List<TooltipProperty> tooltips)
      => tooltips[3];

  }

}