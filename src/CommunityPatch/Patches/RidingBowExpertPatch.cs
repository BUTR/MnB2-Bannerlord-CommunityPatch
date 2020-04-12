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

  public class RidingBowExpertPatch : AgentWeaponEquippedPatch<RidingBowExpertPatch> {

    private static readonly MethodInfo PatchMethodInfo = typeof(RidingBowExpertPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject _bowExpert;

    public override void Reset() {
      _bowExpert = PerkObject.FindFirst(perk => perk.Name.GetID() == "cKTeea27");
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
      if (weapon.ItemUsage == "long_bow") // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
        WeaponComponentDataItemUsageMethod
          .Invoke(weapon, new[] {HeroHasPerk(character, _bowExpert) ? "bow" : weapon.ItemUsage});
    }

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
        if (weapon.ItemUsageIndex != MBItem.GetItemUsageIndex("long_bow")
          || !HeroHasPerk(__instance.Character, _bowExpert))
          continue;

        var updatedWeapon = weapon;
        updatedWeapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
        weaponStatsData[i] = updatedWeapon;
      }
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
      bool isWieldedOnSpawn)
      => ActivePatch.OnWeaponEquipped(__instance, equipmentSlot, ref weaponData, ref weaponStatsData, ref ammoWeaponData, ref ammoWeaponStatsData, weaponEntity);

  }

}