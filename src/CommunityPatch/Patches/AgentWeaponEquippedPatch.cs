using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public abstract class AgentWeaponEquippedPatch : PatchBase<AgentWeaponEquippedPatch> {

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgentWeaponEquippedPatch).GetMethod(nameof(PostFix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override bool Applied { get; protected set; }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      if (Applied) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      return _IsApplicable(game);
    }

    private static void PostFix(ref Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      ref GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn)
      => ActivePatch.Apply(__instance, equipmentSlot, ref weaponData, ref weaponStatsData, ref ammoWeaponData, ref ammoWeaponStatsData, weaponEntity);

    protected abstract bool _IsApplicable(Game game);

    protected abstract void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity);

    public static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      var heroObject = character.IsHero ? (CharacterObject) character : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }

    public override void Reset() {
    }

  }

}