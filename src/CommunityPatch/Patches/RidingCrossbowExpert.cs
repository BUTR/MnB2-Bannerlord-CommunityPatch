using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches {

  public class RidingCrossbowExpert : AgentWeaponEquippedPatch {
    
    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    protected override bool _IsApplicable(Game game)
      => CommunityPatchSubModule.VersionComparer.GreaterThan(CommunityPatchSubModule.GameVersion, ApplicationVersion.FromString("e1.0.0"));

    protected override void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity) {
      for (int i = 0; i < weaponStatsData.Length; i++) {
        var weapon = weaponStatsData[i];
        if (weapon.WeaponClass == (int) WeaponClass.Crossbow && HeroHasPerk(__instance.Character, CrossbowExpert)) {
          var updatedWeapon = weapon;
          updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~((ulong) WeaponFlags.CantReloadOnHorseback);
          weaponStatsData[i] = updatedWeapon;
        }
      }
    }
  }
}