using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public class SpareArrowsPatch : ExtraAmmoPerksPatch<SpareArrowsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(SpareArrowsPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData) =>
      WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows &&
      hero.GetPerkValue(DefaultPerks.Riding.SpareArrows);
      
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Agent __instance) {
      if (HasMount(__instance)) {
        ApplyPerk(__instance, 3, CanApplyPerk);      
      }
    }
  }
}