using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public static class LootCollectorHelper {

    private static readonly Type LootCollector = Type.GetType("TaleWorlds.CampaignSystem.LootCollector, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

    public static readonly MethodInfo GiveShareOfLootToPartyMethod = LootCollector?.GetMethod("GiveShareOfLootToParty", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo LootCasualtiesMethod = LootCollector?.GetMethod("LootCasualties", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo ConvertLootToGoldMethod = LootCollector?.GetMethod("ConvertLootToGold", NonPublic | Static | DeclaredOnly);

    private static readonly PropertyInfo CasualtiesInBattle = LootCollector?.GetProperty("CasualtiesInBattle", Public | Instance | DeclaredOnly);

    public static TroopRoster GetCasualtiesInBattle(object lootCollector)
      => GetPropertyValue<TroopRoster>(CasualtiesInBattle, lootCollector);

    public static IEnumerable<ItemRosterElement> LootCasualties(object lootCollector, ICollection<TroopRosterElement> shareFromCasualties, float lootFactor)
      => (IEnumerable<ItemRosterElement>) LootCasualtiesMethod.Invoke(lootCollector, new object[] {
        shareFromCasualties,
        lootFactor,
      });

    public static int ConvertLootToGold(IEnumerable<ItemRosterElement> lootedItemsRecoveredFromCasualties)
      => (int) ConvertLootToGoldMethod.Invoke(null, new object[] {
        lootedItemsRecoveredFromCasualties
      });

    private static T GetPropertyValue<T>(PropertyInfo propertyInfo, object lootCollector)
      => (T) propertyInfo.GetValue(lootCollector);

  }

}