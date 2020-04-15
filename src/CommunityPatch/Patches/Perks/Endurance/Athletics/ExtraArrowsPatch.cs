using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches.Perks.Endurance.Athletics {

  public class ExtraArrowsPatch : ExtraAmmoPerksPatch<ExtraArrowsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraArrowsPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData) => 
      WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows && 
      hero.GetPerkValue(DefaultPerks.Athletics.ExtraArrows);
      
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Agent __instance) {
      if (!HasMount(__instance)) {
        ApplyPerk(__instance, 2, CanApplyPerk);      
      }
    }
  }
}