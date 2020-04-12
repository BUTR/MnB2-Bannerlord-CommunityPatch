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

    internal static readonly byte[][] HashesAddWeaponItemFlags = {
      new byte[] {
        // e1.1.0.224785
        0x04, 0x32, 0x6C, 0x40, 0xC4, 0xAA, 0x96, 0xB9,
        0xDA, 0x8E, 0xDA, 0x13, 0xBC, 0xC5, 0x5C, 0xE9,
        0xF8, 0x76, 0x43, 0xE7, 0x43, 0x86, 0x97, 0xF0,
        0x75, 0x46, 0xE5, 0x6A, 0x0C, 0x5A, 0xCB, 0x50
      }
    };

    internal static readonly byte[][] HashesWeaponEquipped = {
      new byte[] {
        // e1.1.0.224785
        0x94, 0x3C, 0x6C, 0x69, 0x31, 0x39, 0xBD, 0xB0,
        0x7D, 0xAF, 0x67, 0x3E, 0x8F, 0xDC, 0xF1, 0x96,
        0x08, 0x3E, 0x13, 0xB0, 0xAA, 0x4A, 0x48, 0x8B,
        0x65, 0xBB, 0x3C, 0x5B, 0x5D, 0xFD, 0x44, 0xD1
      }
    };

  }

  public abstract class AgentWeaponEquippedPatch<TPatch> : PatchBase<TPatch> where TPatch : AgentWeaponEquippedPatch<TPatch> {

    internal static byte[][] HashesItemMenuVmAddWeaponItemFlags => HashesItemMenuVmAddWeaponItemFlags;

    internal static byte[][] HashesAgentWeaponEquipped => HashesAgentWeaponEquipped;

    internal static MethodInfo AgentWeaponEquipped => AgentWeaponEquippedPatch.AgentWeaponEquipped;

    internal static MethodInfo ItemMenuVmAddWeaponItemFlags => AgentWeaponEquippedPatch.ItemMenuVmAddWeaponItemFlags;

    internal static FieldInfo ItemMenuVmCharacterField => AgentWeaponEquippedPatch.ItemMenuVmCharacterField;

    internal static MethodInfo WeaponComponentDataItemUsageMethod => AgentWeaponEquippedPatch.WeaponComponentDataItemUsageMethod;

    public override bool Applied { get; protected set; }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(ItemMenuVmAddWeaponItemFlags, new HarmonyMethod(ItemMenuVmAddWeaponItemFlagsPrefix));
      CommunityPatchSubModule.Harmony.Patch(AgentWeaponEquipped, new HarmonyMethod(CallWeaponEquippedPrefix));
      Applied = true;
    }

    private MethodInfo _callWeaponEquippedPrefix;

    private MethodInfo CallWeaponEquippedPrefix
      => _callWeaponEquippedPrefix ??= GetType()
        .GetMethod("CallWeaponEquippedPrefix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private MethodInfo _itemMenuVmAddWeaponItemFlagsPrefix;

    private MethodInfo ItemMenuVmAddWeaponItemFlagsPrefix
      => _itemMenuVmAddWeaponItemFlagsPrefix ??= GetType()
        .GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return ItemMenuVmAddWeaponItemFlags;
      yield return AgentWeaponEquipped;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo1 = Harmony.GetPatchInfo(ItemMenuVmAddWeaponItemFlags);
      if (AlreadyPatchedByOthers(patchInfo1))
        return false;

      var patchInfo2 = Harmony.GetPatchInfo(AgentWeaponEquipped);
      if (AlreadyPatchedByOthers(patchInfo2))
        return false;

      var hash1 = ItemMenuVmAddWeaponItemFlags.MakeCilSignatureSha256();
      if (!hash1.MatchesAnySha256(HashesItemMenuVmAddWeaponItemFlags))
        return false;

      var hash2 = AgentWeaponEquipped.MakeCilSignatureSha256();
      if (!hash1.MatchesAnySha256(HashesAgentWeaponEquipped))
        return false;

      return !AppliesToVersion(game);
    }

    // ReSharper disable once InconsistentNaming

    protected abstract bool AppliesToVersion(Game game);

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected abstract void OnWeaponEquipped(Agent __instance,
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