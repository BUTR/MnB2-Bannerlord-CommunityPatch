using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [HarmonyPatch(typeof(DefaultPerks), "InitializePerks")]
  public class NomadicTraditionsEffectPatch {

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(DefaultPerks __instance) {
      var nomadicTraditions = typeof(DefaultPerks).GetField("RidingNomadicTraditions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
      var primaryBonus = typeof(PerkObject).GetProperty("PrimaryBonus", BindingFlags.Instance | BindingFlags.Public);
      primaryBonus.SetValue(nomadicTraditions, 0.3f);
    }
  }
}