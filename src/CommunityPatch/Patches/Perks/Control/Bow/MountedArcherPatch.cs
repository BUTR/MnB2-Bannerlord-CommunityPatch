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

namespace CommunityPatch.Patches.Perks.Control.Bow {

  public sealed class MountedArcherPatch : AgentWeaponEquippedPatch<MountedArcherPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(MountedArcherPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject _mountedArcher;

    public override void Reset() {
      _mountedArcher = PerkObject.FindFirst(perk => perk.Name.GetID() == "eU0uANvZ");
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
      if (weapon.ItemUsage == "long_bow")
        WeaponComponentDataItemUsageMethod
          .Invoke(weapon, new object[] {HeroHasPerk(character, _mountedArcher) ? "bow" : weapon.ItemUsage});
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
        if (weapon.ItemUsageIndex != MBItem.GetItemUsageIndex("long_bow")
          || !HeroHasPerk(__instance.Character, _mountedArcher))
          continue;

        var updatedWeapon = weapon;
        updatedWeapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
        weaponStatsData[i] = updatedWeapon;
      }
    }

  }

}