using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class CrossbowExpert : AgentWeaponEquippedPatch<CrossbowExpert> {

    private static readonly MethodInfo PatchMethodInfo = typeof(CrossbowExpert).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    public CrossbowExpert() : base("T4fREm7U") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      base.Apply(game);
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      foreach (var mb in base.GetMethodsChecked())
        yield return mb;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static void Prefix(ItemMenuVM __instance, MBBindingList<ItemFlagVM> list, ref WeaponComponentData weapon) {
      var character = ItemMenuVmCharacterGetter(__instance);
      // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
      if (weapon.WeaponClass == WeaponClass.Crossbow && HeroHasPerk(character, ActivePatch.Perk))
        weapon.WeaponFlags &= ~WeaponFlags.CantReloadOnHorseback;
    }

    protected override bool AppliesToVersion(Game game)
      => CommunityPatchSubModule.VersionComparer.GreaterThan(CommunityPatchSubModule.GameVersion, ApplicationVersion.FromString("e1.0.0"));

    [UsedImplicitly]
    // workaround for https://github.com/pardeike/Harmony/issues/286
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
        ref var weapon = ref weaponStatsData[i];
        if (weapon.WeaponClass != (int) WeaponClass.Crossbow
          || !HeroHasPerk(__instance.Character, ActivePatch.Perk))
          continue;

        var updatedWeapon = weapon;
        updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~(ulong) WeaponFlags.CantReloadOnHorseback;
        weaponStatsData[i] = updatedWeapon;
      }
    }

  }

}