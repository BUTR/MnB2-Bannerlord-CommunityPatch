#if !AFTER_E1_4_3

using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Endurance.Athletics {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public class ExtraThrowingWeapons : ExtraAmmoPerksPatch<ExtraThrowingWeapons> {

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraThrowingWeapons).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public ExtraThrowingWeapons() : base("WEcJkDSD") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(ExtraAmmoPerksPatch.TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData)
      => WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Thrown &&
        hero.GetPerkValue(ActivePatch.Perk);

    private static void Postfix(Agent __instance) {
      if (!HasMount(__instance))
        ApplyPerk(__instance, 1, CanApplyPerk);
    }

  }

}

#endif