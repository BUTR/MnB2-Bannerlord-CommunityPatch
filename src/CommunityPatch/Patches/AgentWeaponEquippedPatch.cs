using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public abstract class AgentWeaponEquippedPatch : PatchBase<AgentWeaponEquippedPatch> {

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent)
      .GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgentWeaponEquippedPatch)
      .GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Prefix(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn) {
      if (weaponStatsData == null)
        return;

      var patch = ActivePatch;
      for (var i = 0; i < weaponStatsData.Length; i++) {
        ref var weapon = ref weaponStatsData[i];
        patch.Apply(__instance, ref weapon);
      }
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

    protected static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      var heroObject = character.IsHero ? ((CharacterObject) character) : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }

    protected abstract void Apply(Agent agent, ref WeaponStatsData weapon);

    public override void Reset() {
    }

  }

}