using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches {

  public class RidingCrossbowExpert : AgentWeaponEquippedPatch<RidingCrossbowExpert> {

    private static readonly MethodInfo TargetMethodInfo = typeof(ItemMenuVM).GetMethod("AddWeaponItemFlags", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(RidingCrossbowExpert).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      base.Apply(game);
    }

    private static void Prefix(ItemMenuVM __instance, MBBindingList<ItemFlagVM> list, ref WeaponComponentData weapon) {
      var character = (BasicCharacterObject) typeof(ItemMenuVM).GetField("_character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
      if (weapon.WeaponClass == WeaponClass.Crossbow) // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
        weapon.WeaponFlags = HeroHasPerk(character, CrossbowExpert) ? weapon.WeaponFlags & ~WeaponFlags.CantReloadOnHorseback : weapon.WeaponFlags;
    }

    protected override bool _IsApplicable(Game game)
      => CommunityPatchSubModule.VersionComparer.GreaterThan(CommunityPatchSubModule.GameVersion, ApplicationVersion.FromString("e1.0.0"));

    protected override void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity) {
      for (var i = 0; i < (weaponStatsData?.Length ?? 0); i++) {
        var weapon = weaponStatsData[i];
        if (weapon.WeaponClass == (int) WeaponClass.Crossbow && HeroHasPerk(__instance.Character, CrossbowExpert)) {
          var updatedWeapon = weapon;
          updatedWeapon.WeaponFlags = weapon.WeaponFlags & ~(ulong) WeaponFlags.CantReloadOnHorseback;
          weaponStatsData[i] = updatedWeapon;
        }
      }
    }

  }

}