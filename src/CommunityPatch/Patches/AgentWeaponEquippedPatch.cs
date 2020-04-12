using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public static class AgentWeaponEquippedPatch {

    internal static readonly MethodInfo AgentWeaponEquipped = typeof(Agent)
      .GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    internal static readonly MethodInfo ItemMenuVmAddWeaponItemFlags = typeof(ItemMenuVM).GetMethod("AddWeaponItemFlags", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    internal static readonly FieldInfo ItemMenuVmCharacterField = typeof(ItemMenuVM).GetField("_character", BindingFlags.Instance | BindingFlags.NonPublic);

    internal static readonly MethodInfo WeaponComponentDataItemUsageMethod = typeof(WeaponComponentData)
      .GetMethod("set_ItemUsage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

  }

  public abstract class AgentWeaponEquippedPatch<TPatch> : PatchBase<TPatch> where TPatch : AgentWeaponEquippedPatch<TPatch> {

    private MethodInfo PatchMethodInfo => GetType()
      .GetMethod("CallWeaponEquippedPrefix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    internal static MethodInfo AgentWeaponEquipped => AgentWeaponEquippedPatch.AgentWeaponEquipped;

    internal static MethodInfo ItemMenuVmAddWeaponItemFlags => AgentWeaponEquippedPatch.ItemMenuVmAddWeaponItemFlags;

    internal static FieldInfo ItemMenuVmCharacterField => AgentWeaponEquippedPatch.ItemMenuVmCharacterField;

    internal static MethodInfo WeaponComponentDataItemUsageMethod => AgentWeaponEquippedPatch.WeaponComponentDataItemUsageMethod;

    public override bool Applied { get; protected set; }

    public static bool AgentWeaponEquippedPatched { get; private set; }

    public override void Apply(Game game) {
      if (AgentWeaponEquippedPatched) return;

      CommunityPatchSubModule.Harmony.Patch(AgentWeaponEquipped, new HarmonyMethod(PatchMethodInfo));
      AgentWeaponEquippedPatched = true;
      Applied = true;
    }
    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AgentWeaponEquipped;
    }


    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(AgentWeaponEquipped);

      return !AlreadyPatchedByOthers(patchInfo)
        && AppliesToVersion(game);
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static void WeaponEquippedPrefix(ref Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      ref GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn)
      => ActivePatch.Apply(__instance, equipmentSlot, ref weaponData, ref weaponStatsData, ref ammoWeaponData, ref ammoWeaponStatsData, weaponEntity);

    protected abstract bool AppliesToVersion(Game game);

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected abstract void Apply(Agent __instance,
      EquipmentIndex equipmentSlot,
      ref WeaponData weaponData,
      ref WeaponStatsData[] weaponStatsData,
      ref WeaponData ammoWeaponData,
      ref WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity);

    protected static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk) {
      var heroObject = character.IsHero ? (CharacterObject) character : null;
      return heroObject?.GetPerkValue(perk) ?? false;
    }

    public override void Reset() {
    }

  }

}