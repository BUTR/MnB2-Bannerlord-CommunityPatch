using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class ExtraAmmoPerksPatch : PatchBase<ExtraAmmoPerksPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod("InitializeMissionEquipment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ExtraAmmoPerksPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);



    public override void Reset(){}

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      /*
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
          // e.1.0.7
          0x4B, 0x26, 0xD4, 0x1E, 0xF7, 0xCF, 0x5B, 0x15,
          0xE1, 0x24, 0x74, 0x8D, 0xE9, 0x46, 0x36, 0x80,
          0x6A, 0x91, 0x65, 0x5D, 0x7A, 0x6C, 0x3F, 0x43,
          0xD2, 0x7B, 0x80, 0xA7, 0x3E, 0xF0, 0x10, 0xF6
        })
        || hash.SequenceEqual(new byte[] {
          // e.1.0.8
          0xB5, 0xEE, 0x39, 0xE3, 0xF3, 0xDF, 0x4C, 0xE2,
          0xC0, 0xAF, 0xD3, 0x1B, 0x5F, 0x6D, 0x36, 0x11,
          0x76, 0x0B, 0xA3, 0xA4, 0x45, 0xB1, 0xF8, 0x57,
          0x72, 0xA3, 0x60, 0x08, 0xC4, 0x44, 0x22, 0x89
        })
        || hash.SequenceEqual(new byte[] {
          // e.1.0.9
          0x1a,0xb6,0xf4,0xca,0xac,0xb6,0x6a,0x88,
          0x93,0xf4,0xde,0x2b,0x5b,0xa2,0x4a,0x45,
          0x64,0xc6,0x26,0x37,0x69,0x7c,0x03,0x7c,
          0xf7,0x53,0x85,0xfc,0x14,0x54,0x5d,0x72
        });
        */
      return true;
       
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