using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  public static class SiegeTooltipHelper {

    private const string WallDamageLabel = "Projectile Wall Damage";

    public static readonly MethodInfo TargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.SandBoxUIHelper, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetSiegeEngineInProgressTooltip", NonPublic | Public | Instance | Static | DeclaredOnly);

    public static readonly byte[][] TooltipHashes = {
      new byte[] {
        // e1.1.0.225190
        0x68, 0xEE, 0xE7, 0xAD, 0xA8, 0x04, 0xCC, 0x5C,
        0x5E, 0xBA, 0xB6, 0x3D, 0x93, 0xE0, 0x91, 0xC9,
        0x32, 0x6F, 0x65, 0x2E, 0xAF, 0xBF, 0xF3, 0xBD,
        0x51, 0x75, 0x34, 0x1A, 0xDC, 0x0D, 0xD3, 0x25
      },
      new byte[] {
        0xA4, 0x93, 0x88, 0xD0, 0x7F, 0x04, 0x72, 0x3D,
        0x40, 0xD6, 0xB3, 0x14, 0xED, 0xC7, 0x36, 0xDF,
        0xBF, 0x97, 0x0C, 0x56, 0x7C, 0x2D, 0x23, 0x01,
        0x7E, 0x1A, 0xBC, 0xBD, 0x3F, 0xB5, 0xF5, 0xA5
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

      double.TryParse(rangedDamageProperty.ValueLabel, out var currentRangedDamage);
      var amplifiedRangedDamage = (int) (currentRangedDamage + bonusDamageOnly);
      rangedDamageProperty.ValueLabel = amplifiedRangedDamage.ToString();
    }

    public static void UpdateMaxHpTooltip(List<TooltipProperty> tooltips, float bonusHp) {
      var property = FindMaxHpTooltipProperty(tooltips);
      if (property == null) return;

      double.TryParse(property.ValueLabel, out var currentHp);
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

      double.TryParse(wallDamageProperty.ValueLabel, out var currentWallDamage);
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