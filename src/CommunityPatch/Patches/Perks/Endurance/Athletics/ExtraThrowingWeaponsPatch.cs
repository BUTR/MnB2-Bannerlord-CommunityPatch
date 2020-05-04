using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Endurance.Athletics {

  public class ExtraThrowingWeapons : ExtraAmmoPerksPatch<ExtraThrowingWeapons> {

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraThrowingWeapons).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public ExtraThrowingWeapons() : base("WEcJkDSD") {}

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData) =>
      WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Thrown &&
      hero.GetPerkValue(ActivePatch.Perk);

    
    private static void Postfix(Agent __instance) {
      if (!HasMount(__instance))
        ApplyPerk(__instance, 1, CanApplyPerk);
    }

  }

}