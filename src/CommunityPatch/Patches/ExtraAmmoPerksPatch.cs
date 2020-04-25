using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public abstract class ExtraAmmoPerksPatch<TPatch> : PatchBase<TPatch> where  TPatch : ExtraAmmoPerksPatch<TPatch> {

    public override bool Applied { get; protected set; }

    protected static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod(nameof(Agent.InitializeMissionEquipment), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xE5, 0x73, 0x59, 0x27, 0xE5, 0xF6, 0x83, 0x53,
        0xAB, 0x6C, 0x24, 0x67, 0xBE, 0x5B, 0xB4, 0x71,
        0xFF, 0x4E, 0x22, 0x8E, 0x30, 0xDE, 0x8A, 0xF4,
        0xFB, 0x87, 0xF4, 0x1C, 0xF1, 0xCB, 0xEE, 0xD0
      }
    };

    private static readonly FieldInfo MaxAmmoField = typeof(MissionWeapon).GetField("_maxDataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

    private static readonly FieldInfo AmmoField = typeof(MissionWeapon).GetField("_dataValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

    private static readonly FieldInfo WeaponSlotsProperty = typeof(MissionEquipment)
      .GetField("_weaponSlots", BindingFlags.NonPublic | BindingFlags.Instance);

    public override void Reset(){}

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }
    
    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    protected static bool HasMount(Agent agent) {
      var item = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
      return item != null && item.HasHorseComponent;
    }

    protected static void ApplyPerk(Agent agent, int ammoAmount, Func<Hero, WeaponComponentData, bool> canApplyPerk) {
      if (!agent.IsHero || !(agent.Character is CharacterObject charObj))
        return;

      var hero = charObj.HeroObject;
      var missionWeapons = (MissionWeapon[]) WeaponSlotsProperty.GetValue(agent.Equipment);

      for (var i = 0; i < missionWeapons.Length; i++) {
        if (missionWeapons[i].Weapons.IsEmpty())
          continue;

        var weaponComponentData = missionWeapons[i].Weapons[0];
        if (weaponComponentData == null || !canApplyPerk(hero, weaponComponentData))
          continue;

        var newMaxAmmo = (short) ((short) MaxAmmoField.GetValue(missionWeapons[i]) + ammoAmount);
        object boxed = missionWeapons[i];
        AmmoField.SetValue(boxed, newMaxAmmo);
        MaxAmmoField.SetValue(boxed, newMaxAmmo);
        missionWeapons[i] = (MissionWeapon) boxed;
      }
    }
  }
}