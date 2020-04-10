using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.Patches.AgentWeaponEquippedPatch;

namespace CommunityPatch.Patches {

  public class BowMountedArcherPatch : AgentWeaponEquippedPatch {
    private static PerkObject MountedArcher => PerkObject.FindFirst(perk => perk.Name.GetID() == "cKTeea27");

    protected override void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity) {

      for (int i = 0; i < weaponStatsData.Length; i++) {
        var weapon = weaponStatsData[i];
        if (weapon.ItemUsageIndex == MBItem.GetItemUsageIndex("long_bow") && HeroHasPerk(__instance.Character, MountedArcher)) {
          var updatedWeapon = weapon;
          updatedWeapon.ItemUsageIndex = MBItem.GetItemUsageIndex("bow");
          weaponStatsData[i] = updatedWeapon;
        }
      }
    }

    protected override bool _IsApplicable(Game game)
      => CommunityPatchSubModule.VersionComparer.GreaterThan(CommunityPatchSubModule.GameVersion, ApplicationVersion.FromString("e1.0.0"));
  }
}