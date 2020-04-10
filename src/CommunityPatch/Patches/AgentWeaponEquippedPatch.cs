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

  public class AgentWeaponEquippedPatch : PatchBase<AgentWeaponEquippedPatch> {

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgentWeaponEquippedPatch).GetMethod(nameof(PostFix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override bool Applied { get; protected set; }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        null,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      if (Applied) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

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

    private static void PostFix(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn) {

      var updatedWeaponStatsData = weaponStatsData;
      if (weaponStatsData != null) {
        updatedWeaponStatsData = weaponStatsData        
          .Select(weapon => CrossbowCavalryCrossbowExpertPerksPatch.Apply(__instance, weapon))
          .ToArray();
      }
      
      FieldInfo IMBAgent = typeof(MBAPI).GetField("IMBAgent", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
      MethodInfo unmanagedCall = IMBAgent.FieldType.GetMethod("WeaponEquipped");
      UIntPtr agentPointer = (UIntPtr) typeof(Agent)
        .GetMethod("get_Pointer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
        .Invoke(__instance, new object[] { });
      UIntPtr weaponEntityPointer = weaponEntity != null
        ? (UIntPtr) typeof(NativeObject)
            .GetMethod("get_Pointer", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(weaponEntity, new object[] { })
        : UIntPtr.Zero;

      unmanagedCall.Invoke(IMBAgent.GetValue(null), new object[] {
        agentPointer,
        (int) equipmentSlot,
        weaponData,
        updatedWeaponStatsData,
        weaponStatsData?.Length ?? 0,
        ammoWeaponData,
        ammoWeaponStatsData,
        ammoWeaponStatsData?.Length ?? 0,
        weaponEntityPointer,
        removeOldWeaponFromScene,
        isWieldedOnSpawn
      });
    }

    public static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      CharacterObject heroObject = character.IsHero ? ((CharacterObject) character) : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }

    public override void Reset() {
    }
  }
}