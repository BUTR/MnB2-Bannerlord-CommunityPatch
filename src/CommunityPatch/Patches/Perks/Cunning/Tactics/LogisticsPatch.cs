using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class LogisticsPatch : ExtraAmmoPerksPatch<LogisticsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(LogisticsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Agent __instance) {
      if (MyLeaderIsIntoLogistics(__instance))
        ApplyPerkToAgent(__instance, CanApplyPerk, CalculateAmmoIncrease);
    }

    private static bool MyLeaderIsIntoLogistics(Agent agent) {
      var leader = (
        (agent.Origin as PartyGroupAgentOrigin)?.Party
        ?? (agent.Origin as SimpleAgentOrigin)?.Party
      )?.Leader;

      if (leader == null || leader == agent.Character)
        return false;

      return leader.GetPerkValue(DefaultPerks.Tactics.Logistics);
    }

    private static bool CanApplyPerk(Agent agent, WeaponComponentData weaponComponentData) {
      var weaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass);

      return weaponClass == ItemObject.ItemTypeEnum.Arrows ||
        weaponClass == ItemObject.ItemTypeEnum.Bolts ||
        weaponClass == ItemObject.ItemTypeEnum.Thrown;
    }

    private static int CalculateAmmoIncrease(short maxAmmo)
      => (int) (maxAmmo * DefaultPerks.Tactics.Logistics.PrimaryBonus);

  }

}