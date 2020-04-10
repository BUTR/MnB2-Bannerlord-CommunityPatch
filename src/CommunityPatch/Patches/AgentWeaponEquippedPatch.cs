using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public abstract class AgentWeaponEquippedPatch : PatchBase<AgentWeaponEquippedPatch> {

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent)
      .GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    // ReSharper disable once InconsistentNaming
    private static readonly FieldInfo IMBAgentField = typeof(MBAPI)
      .GetField("IMBAgent", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo GetWeaponEquipped = IMBAgentField.FieldType.GetMethod("WeaponEquipped");

    private static readonly MethodInfo GetAgentPointer = typeof(Agent)
      .GetMethod("get_Pointer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo GetNativeObjectPointer = typeof(NativeObject)
      .GetMethod("get_Pointer", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgentWeaponEquippedPatch)
      .GetMethod(nameof(PostFix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        null,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void PostFix(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn) {
      if (weaponStatsData != null) {
        var patch = ActivePatch;
        for (var i = 0; i < weaponStatsData.Length; i++) {
          ref var weapon = ref weaponStatsData[i];
          patch.Apply(__instance, ref weapon);
        }
      }

      var agentPointer = (UIntPtr) GetAgentPointer.Invoke(__instance, new object[0]);
      var weaponEntityPointer = weaponEntity != null
        ? (UIntPtr) GetNativeObjectPointer.Invoke(weaponEntity, new object[0])
        : UIntPtr.Zero;

      GetWeaponEquipped.Invoke(IMBAgentField.GetValue(null), new object[] {
        agentPointer,
        (int) equipmentSlot,
        weaponData,
        weaponStatsData,
        weaponStatsData?.Length ?? 0,
        ammoWeaponData,
        ammoWeaponStatsData,
        ammoWeaponStatsData?.Length ?? 0,
        weaponEntityPointer,
        removeOldWeaponFromScene,
        isWieldedOnSpawn
      });
    }

    public override bool Applied { get; protected set; }

    public override bool IsApplicable(Game game) {
      if (Applied) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      // TODO: game version check, skip il hash check?

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();

      return hash.SequenceEqual(new byte[] {
        0x07, 0x67, 0x8D, 0xA3, 0xE7, 0x7C, 0x15, 0xD6,
        0xF6, 0xE9, 0xAB, 0x97, 0xDF, 0x80, 0xBB, 0x5C,
        0x0B, 0x04, 0x39, 0x34, 0x8D, 0x69, 0x98, 0xED,
        0xE7, 0xA9, 0x48, 0xED, 0x72, 0x4F, 0x77, 0xAC
      });
    }

    public static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      var heroObject = character.IsHero ? ((CharacterObject) character) : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }

    public abstract void Apply(Agent agent, ref WeaponStatsData weapon);

    public override void Reset() {
    }

  }

}