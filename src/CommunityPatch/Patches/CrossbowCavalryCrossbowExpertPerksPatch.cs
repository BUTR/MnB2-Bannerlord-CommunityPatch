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

  public class CrossbowCavalryCrossbowExpertPerksPatch : PatchBase<CrossbowCavalryCrossbowExpertPerksPatch> {

    public override bool Applied { get; protected set; }

    private static PerkObject CrossbowCavalry => PerkObject.FindFirst(perk => perk.Name.GetID() == "sHXLjzCb");
    private static PerkObject CrossbowExpert => PerkObject.FindFirst(perk => perk.Name.GetID() == "T4fREm7U");

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(CrossbowCavalryCrossbowExpertPerksPatch).GetMethod(nameof(PostFix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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
        FieldInfo IMBAgent = typeof(MBAPI).GetField("IMBAgent", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        MethodInfo unmanagedCall = IMBAgent.FieldType.GetMethod("WeaponEquipped");
        UIntPtr agentPointer = (UIntPtr) typeof(Agent)
          .GetMethod("get_Pointer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
          .Invoke(__instance, new object[]{});
        UIntPtr weaponEntityPointer = weaponEntity != null 
          ? (UIntPtr) typeof(NativeObject).GetMethod("get_Pointer", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(weaponEntity, new object[]{})
          : UIntPtr.Zero;

        WeaponStatsData[] updatedWeaponStatsData = null;
        if (weaponStatsData != null) {
          updatedWeaponStatsData = new WeaponStatsData[weaponStatsData.Length];
          
          for (int i = 0; i < weaponStatsData.Length; i ++) {
            if (weaponStatsData[i].WeaponClass == (int) WeaponClass.Crossbow 
              && (HeroHasPerk(__instance.Character, CrossbowCavalry) || HeroHasPerk(__instance.Character, CrossbowExpert))) {
                var newStatsData = weaponStatsData[i];
                newStatsData.WeaponFlags = weaponStatsData[i].WeaponFlags & ~((ulong) WeaponFlags.CantReloadOnHorseback);
                updatedWeaponStatsData[i] = newStatsData;
            }
            else {
              updatedWeaponStatsData[i] = weaponStatsData[i];
            }
          }
        }

        unmanagedCall.Invoke(IMBAgent.GetValue(null), new object[] {
          agentPointer, 
          (int) equipmentSlot, 
          weaponData, 
          updatedWeaponStatsData, 
          updatedWeaponStatsData != null ? weaponStatsData.Length : 0, 
          ammoWeaponData,
          ammoWeaponStatsData, 
          ammoWeaponStatsData != null ? ammoWeaponStatsData.Length : 0, 
          weaponEntityPointer, 
          removeOldWeaponFromScene, 
          isWieldedOnSpawn
        });
        
    }

    private static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      CharacterObject heroObject = character.IsHero ? ((CharacterObject) character) : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }
    
    public override void Reset() {}
    

  }

}