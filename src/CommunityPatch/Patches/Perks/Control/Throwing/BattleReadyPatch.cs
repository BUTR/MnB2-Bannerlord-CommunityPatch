using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Control.Throwing {

  public sealed class BattleReadyPatch : ExtraAmmoPerksPatch<BattleReadyPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(BattleReadyPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    static bool CanApplyPerk(Hero hero, WeaponComponentData weaponComponentData) =>
      WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass) == ItemObject.ItemTypeEnum.Thrown &&
      hero.GetPerkValue(DefaultPerks.Throwing.BattleReady);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Agent __instance) => ApplyPerk(__instance, 2, CanApplyPerk);

  }

}