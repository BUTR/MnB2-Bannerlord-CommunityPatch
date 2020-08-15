using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public class SpareArrowsPatch : ExtraAmmoPerksPatch<SpareArrowsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(SpareArrowsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public SpareArrowsPatch() : base("Bj6la71T") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(ExtraAmmoPerksPatch.TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData)
      => WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Arrows &&
        hero.GetPerkValue(ActivePatch.Perk);

    private static void Postfix(Agent __instance) {
      if (HasMount(__instance))
        ApplyPerk(__instance, 3, CanApplyPerk);
    }

  }

}