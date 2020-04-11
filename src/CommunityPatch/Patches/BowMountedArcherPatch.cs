using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch.Patches {

  public class BowMountedArcherPatch : AgentWeaponEquippedPatch<BowMountedArcherPatch> {

    private static readonly MethodInfo TargetMethodInfo = typeof(ItemMenuVM).GetMethod("AddWeaponItemFlags", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(BowMountedArcherPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static PerkObject MountedArcher => PerkObject.FindFirst(perk => perk.Name.GetID() == "eU0uANvZ");

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      base.Apply(game);
    }

    private static void Prefix(ItemMenuVM __instance, MBBindingList<ItemFlagVM> list, ref WeaponComponentData weapon) {
      var character = (BasicCharacterObject) typeof(ItemMenuVM).GetField("_character", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
      if (weapon.ItemUsage == "long_bow") // Make sure we're always using the correct value, in case this overwrites some shared WeaponComponentData
        typeof(WeaponComponentData)
          .GetMethod("set_ItemUsage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
          .Invoke(weapon, new[] {HeroHasPerk(character, MountedArcher) ? "bow" : weapon.ItemUsage});
    }

    protected override void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity) {
      for (var i = 0; i < (weaponStatsData?.Length ?? 0); i++) {
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