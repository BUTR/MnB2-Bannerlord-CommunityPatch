using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public sealed class ExtraAmmoPerksPatch : PatchBase<ExtraAmmoPerksPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod(nameof(Agent.InitializeMissionEquipment), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraAmmoPerksPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xE5, 0x73, 0x59, 0x27, 0xE5, 0xF6, 0x83, 0x53,
        0xAB, 0x6C, 0x24, 0x67, 0xBE, 0x5B, 0xB4, 0x71,
        0xFF, 0x4E, 0x22, 0x8E, 0x30, 0xDE, 0x8A, 0xF4,
        0xFB, 0x87, 0xF4, 0x1C, 0xF1, 0xCB, 0xEE, 0xD0
      }
    };

    public override void Reset(){}

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }
    
    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    static short ApplyArrowPerks(Hero hero, bool hasMount) {
      short extraAmmo = 0;

      if (hero.GetPerkValue(DefaultPerks.Bow.LargeQuiver)) {
        extraAmmo += 3;
      }

      if (hero.GetPerkValue(DefaultPerks.Bow.BattleEquipped)) {
        extraAmmo += 6;
      }
      
      if (hasMount) {
        if (hero.GetPerkValue(DefaultPerks.Riding.SpareArrows)) {
          extraAmmo += 3;
        }
      }
      else {
        if (hero.GetPerkValue(DefaultPerks.Athletics.ExtraArrows)) {
          extraAmmo += 2;
        }
      }
      return extraAmmo;
    }
    
    static short ApplyThrowingAmmoPerks(Hero hero, bool hasMount) {
      short extraAmmo = 0;

      if (hero.GetPerkValue(DefaultPerks.Throwing.FullyArmed)) {
        extraAmmo += 1;
      }

      if (hero.GetPerkValue(DefaultPerks.Throwing.BattleReady)) {
        extraAmmo += 2;
      }
      
      if (hasMount) {
        if (hero.GetPerkValue(DefaultPerks.Riding.SpareThrowingWeapon)) {
          extraAmmo += 1;
        }
      }
      else {
        if (hero.GetPerkValue(DefaultPerks.Athletics.ExtraThrowingWeapons)) {
          extraAmmo += 1;
        }
      }
      return extraAmmo;
    }

    private static void Postfix(Agent __instance) {
      if (!__instance.IsHero) {
        return;
      }

      if (__instance.Character is CharacterObject charObj) {
        var property = typeof(MissionEquipment)
          .GetField("_weaponSlots", BindingFlags.NonPublic | BindingFlags.Instance);
        var missionWeapons = (MissionWeapon[]) property.GetValue(__instance.Equipment);

        //At this point agent.HasMount wasn't initialized, and trying to modify ammo in later functions causes problems
        var item = __instance.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
        var hasMount = item != null && item.HasHorseComponent;

        for (var i = 0; i < missionWeapons.Length; i++) {
          if (!missionWeapons[i].Weapons.IsEmpty()) {
            var weaponComponentData = missionWeapons[i].Weapons[0];
            if (weaponComponentData != null) {
              var hero = charObj.HeroObject;
              short extraAmmo = 0;

              if (weaponComponentData.WeaponClass == WeaponClass.Arrow) {
                extraAmmo = ApplyArrowPerks(hero, hasMount);
              }
              else if (weaponComponentData.WeaponClass == WeaponClass.ThrowingAxe ||
                weaponComponentData.WeaponClass == WeaponClass.ThrowingKnife ||
                weaponComponentData.WeaponClass == WeaponClass.Javelin) {
                extraAmmo = ApplyThrowingAmmoPerks(hero, hasMount);
              }

              if (extraAmmo > 0) {
                var maxAmmoField = typeof(MissionWeapon).GetField("_maxDataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                var ammoField = typeof(MissionWeapon).GetField("_dataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                var newMaxAmmo = (short) ((short) maxAmmoField.GetValue(missionWeapons[i]) + extraAmmo);
                object boxed = missionWeapons[i];
                ammoField.SetValue(boxed, newMaxAmmo);
                maxAmmoField.SetValue(boxed, newMaxAmmo);
                missionWeapons[i] = (MissionWeapon) boxed;
              }
            }
          }
        }
      }
    }
  }
}