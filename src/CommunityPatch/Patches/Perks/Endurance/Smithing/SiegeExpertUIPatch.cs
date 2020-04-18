using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Endurance.Smithing {

  [HarmonyPatch(typeof(DefaultPerks), "InitializePerks")]
  public class SiegeExpertUIPatch {

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(DefaultPerks __instance) {
      var siegeExpert = typeof(DefaultPerks).GetField("CraftingSiegeExpert", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
      var legendarySmith = typeof(DefaultPerks).GetField("CraftingLegendarySmith",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
      
      var alternativePerk = typeof(PerkObject).GetProperty("AlternativePerk", BindingFlags.Instance | BindingFlags.Public);
      alternativePerk.SetValue(siegeExpert, legendarySmith);
      alternativePerk.SetValue(legendarySmith, siegeExpert);
    }
  }
}