using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;
using static CommunityPatch.Patches.ExtraAmmoPerksPatch;

namespace CommunityPatch.Patches {

  public static class ExtraAmmoPerksPatch {

    public static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod(nameof(Agent.InitializeMissionEquipment), Public | Instance | DeclaredOnly);

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xE5, 0x73, 0x59, 0x27, 0xE5, 0xF6, 0x83, 0x53,
        0xAB, 0x6C, 0x24, 0x67, 0xBE, 0x5B, 0xB4, 0x71,
        0xFF, 0x4E, 0x22, 0x8E, 0x30, 0xDE, 0x8A, 0xF4,
        0xFB, 0x87, 0xF4, 0x1C, 0xF1, 0xCB, 0xEE, 0xD0
      }
    };

    public static readonly FieldInfo MaxAmmoField = typeof(MissionWeapon).GetField("_maxDataValue", Instance | NonPublic | DeclaredOnly);

    public static readonly FieldInfo AmmoField = typeof(MissionWeapon).GetField("_dataValue", Instance | NonPublic | DeclaredOnly);

    public static readonly FieldInfo WeaponSlotsProperty = typeof(MissionEquipment)
      .GetField("_weaponSlots", NonPublic | Instance);

  }

  public abstract class ExtraAmmoPerksPatch<TPatch> : PerkPatchBase<TPatch> where TPatch : ExtraAmmoPerksPatch<TPatch> {

    public static byte[][] Hashes => ExtraAmmoPerksPatch.Hashes;

    public override bool Applied { get; protected set; }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(ExtraAmmoPerksPatch.Hashes))
        return false;

      return base.IsApplicable(game);
    }

    protected static bool HasMount(Agent agent) {
      var item = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
      return item != null && item.HasHorseComponent;
    }

    protected static void ApplyPerk(Agent agent, int ammoAmount, Func<Hero, WeaponComponentData, bool> canApplyPerk) {
      if (!agent.IsHero || !(agent.Character is CharacterObject charObj))
        return;

      var hero = charObj.HeroObject;

      ApplyPerkToAgent(agent,
        (_, weapon) => canApplyPerk(hero, weapon),
        _ => ammoAmount);
    }

    protected static void ApplyPerkToAgent(Agent agent, Func<Agent, WeaponComponentData, bool> canApplyPerk, Func<short, int> getAmmoIncrease) {
      var missionWeapons = (MissionWeapon[]) WeaponSlotsProperty.GetValue(agent.Equipment);

      for (var i = 0; i < missionWeapons.Length; i++) {
        if (missionWeapons[i].Weapons.IsEmpty())
          continue;

        var weaponComponentData = missionWeapons[i].Weapons[0];
        if (weaponComponentData == null || !canApplyPerk(agent, weaponComponentData))
          continue;

        var maxAmmo = (short) MaxAmmoField.GetValue(missionWeapons[i]);
        var newMaxAmmo = (short) (maxAmmo + getAmmoIncrease(maxAmmo));
        object boxed = missionWeapons[i];
        AmmoField.SetValue(boxed, newMaxAmmo);
        MaxAmmoField.SetValue(boxed, newMaxAmmo);
        missionWeapons[i] = (MissionWeapon) boxed;
      }
    }

    protected ExtraAmmoPerksPatch(string perkId) : base(perkId) {
    }

  }

}