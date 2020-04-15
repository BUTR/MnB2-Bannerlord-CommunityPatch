using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches.Perks.Control.Bow {

  public class BattleEquippedPatch : ExtraAmmoPerksPatch<BattleEquippedPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(BattleEquippedPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData) =>
      WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows &&
      hero.GetPerkValue(DefaultPerks.Bow.BattleEquipped);
    
    private static void Postfix(Agent __instance) => ApplyPerk(__instance, 6, CanApplyPerk);

  }
}