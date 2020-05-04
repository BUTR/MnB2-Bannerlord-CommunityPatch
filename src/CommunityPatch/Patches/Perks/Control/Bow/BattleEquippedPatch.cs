using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Control.Bow {

  public class BattleEquippedPatch : ExtraAmmoPerksPatch<BattleEquippedPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(BattleEquippedPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData)
      => WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows &&
        hero.GetPerkValue(ActivePatch.Perk);

    
    private static void Postfix(Agent __instance)
      => ApplyPerk(__instance, 6, CanApplyPerk);

    public BattleEquippedPatch() : base("eU0uANvZ") { }

  }

}