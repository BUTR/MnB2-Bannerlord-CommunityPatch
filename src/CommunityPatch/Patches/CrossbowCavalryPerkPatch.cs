using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.CommunityPatchSubModule;

namespace CommunityPatch.Patches {

  public class CrossbowCavalryPerkPatch : AgentWeaponEquippedPatch<CrossbowCavalryPerkPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(CrossbowCavalryPerkPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject _crossbowCavalry;

    public override void Reset() {
      _crossbowCavalry = PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");
      base.Reset();
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(ItemMenuVmAddWeaponItemFlags, new HarmonyMethod(PatchMethodInfo));
      base.Apply(game);
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      foreach (var mb in base.GetMethodsChecked())
        yield return mb;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(ItemMenuVM __instance, MBBindingList<ItemFlagVM> list, ref WeaponComponentData weapon) {
      var character = (BasicCharacterObject) ItemMenuVmCharacterField.GetValue(__instance);

      // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
      if (weapon.WeaponClass == WeaponClass.Crossbow
        && HeroHasPerk(character, _crossbowCavalry))
        weapon.WeaponFlags &= ~WeaponFlags.CantReloadOnHorseback;
    }

    protected override bool AppliesToVersion(Game game)
      => VersionComparer.GreaterThan(GameVersion, ApplicationVersion.FromString("e1.0.0"));

    // ReSharper disable once InconsistentNaming
    protected override void OnWeaponEquipped(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity) {
      if (weaponStatsData == null)
        return;

      for (var i = 0; i < weaponStatsData.Length; i++) {
        var weapon = weaponStatsData[i];
        if (weapon.WeaponClass != (int) WeaponClass.Crossbow
          || !HeroHasPerk(__instance.Character, _crossbowCavalry))
          continue;

        var updatedWeapon = weapon;
        updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~(ulong) WeaponFlags.CantReloadOnHorseback;
        weaponStatsData[i] = updatedWeapon;
      }
    }

    [UsedImplicitly]
    // workaround for https://github.com/pardeike/Harmony/issues/286
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CallWeaponEquippedPrefix(ref Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      ref GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn)
      => ActivePatch.OnWeaponEquipped(__instance, equipmentSlot, ref weaponData, ref weaponStatsData, ref ammoWeaponData, ref ammoWeaponStatsData, weaponEntity);

  }

}