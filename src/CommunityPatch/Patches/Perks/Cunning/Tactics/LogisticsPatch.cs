using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class LogisticsPatch : ExtraAmmoPerksPatch<LogisticsPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(LogisticsPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public LogisticsPatch() : base("Ilh6hVTu") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(ExtraAmmoPerksPatch.TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    private static void Postfix(Agent __instance) {
      if (ActivePatch.MyLeaderIsIntoLogistics(__instance))
        ApplyPerkToAgent(__instance, CanApplyPerk, ActivePatch.CalculateAmmoIncrease);
    }

    private bool MyLeaderIsIntoLogistics(Agent agent) {
      var leader = (
        (agent.Origin as PartyGroupAgentOrigin)?.Party
        ?? (agent.Origin as SimpleAgentOrigin)?.Party
      )?.Leader;

      if (leader == null || leader == agent.Character)
        return false;

      return leader.GetPerkValue(Perk);
    }

    private static bool CanApplyPerk(Agent agent, WeaponComponentData weaponComponentData) {
      var weaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass);

      return weaponClass == ItemObject.ItemTypeEnum.Arrows ||
        weaponClass == ItemObject.ItemTypeEnum.Bolts ||
        weaponClass == ItemObject.ItemTypeEnum.Thrown;
    }

    private int CalculateAmmoIncrease(short maxAmmo)
      => (int) (maxAmmo * Perk.PrimaryBonus);

  }

}