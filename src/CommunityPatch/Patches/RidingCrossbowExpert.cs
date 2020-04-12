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

namespace CommunityPatch.Patches {

  public class RidingCrossbowExpert : AgentWeaponEquippedPatch<RidingCrossbowExpert> {

    private static readonly MethodInfo PatchMethodInfo = typeof(RidingCrossbowExpert).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject _crossbowExpert;

    public override void Reset() {
      _crossbowExpert = PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");
      base.Reset();
    }

    public override void Apply(Game game) {
      if (Applied) return;

      base.Apply(game);
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      foreach (var mb in base.GetMethodsChecked())
        yield return mb;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(ItemMenuVM __instance, MBBindingList<ItemFlagVM> list, ref WeaponComponentData weapon) {
      var character = (BasicCharacterObject) ItemMenuVmCharacterField.GetValue(__instance);
      if (weapon.WeaponClass == WeaponClass.Crossbow) // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
        weapon.WeaponFlags = HeroHasPerk(character, _crossbowExpert) ? weapon.WeaponFlags & ~WeaponFlags.CantReloadOnHorseback : weapon.WeaponFlags;
    }

    protected override bool AppliesToVersion(Game game)
      => CommunityPatchSubModule.VersionComparer.GreaterThan(CommunityPatchSubModule.GameVersion, ApplicationVersion.FromString("e1.0.0"));

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
      bool isWieldedOnSpawn) {
      if (weaponStatsData == null)
        return;

      for (var i = 0; i < weaponStatsData.Length; i++) {
        var weapon = weaponStatsData[i];
        if (weapon.WeaponClass != (int) WeaponClass.Crossbow
          || !HeroHasPerk(__instance.Character, _crossbowExpert))
          continue;

        var updatedWeapon = weapon;
        updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~(ulong) WeaponFlags.CantReloadOnHorseback;
        weaponStatsData[i] = updatedWeapon;
      }
    }

  }

}