using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
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

    internal static readonly byte[][] HashesItemMenuVmAddWeaponItemFlags = {
      new byte[] {
        // e1.1.0.224785
        0xBD, 0x46, 0x14, 0x60, 0x13, 0xB9, 0x14, 0x9D,
        0xDD, 0x93, 0xBC, 0xBA, 0x06, 0x18, 0x14, 0x9C,
        0x3D, 0x4E, 0x07, 0xE8, 0x40, 0x86, 0x7B, 0x71,
        0x58, 0x62, 0x17, 0x89, 0xCF, 0x06, 0xC2, 0x6F
      }
    };

    internal static readonly byte[][] HashesAgentWeaponEquipped = {
      new byte[] {
        // e1.1.0.224785
        0x29, 0x65, 0x19, 0x98, 0x7C, 0x32, 0x47, 0xB4,
        0x1D, 0x75, 0x29, 0x7E, 0x44, 0xF2, 0xAC, 0x39,
        0x65, 0x5E, 0x8D, 0x5E, 0x41, 0xB3, 0x82, 0x24,
        0xF2, 0x64, 0x00, 0xC3, 0x08, 0xB9, 0x94, 0x46
      }
    };

  }

  public abstract class AgentWeaponEquippedPatch<TPatch> : PatchBase<TPatch> where TPatch : AgentWeaponEquippedPatch<TPatch> {

    internal static byte[][] HashesItemMenuVmAddWeaponItemFlags => AgentWeaponEquippedPatch.HashesItemMenuVmAddWeaponItemFlags;

    internal static byte[][] HashesAgentWeaponEquipped => AgentWeaponEquippedPatch.HashesAgentWeaponEquipped;

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

    public override bool? IsApplicable(Game game) {
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
      if (!hash2.MatchesAnySha256(HashesAgentWeaponEquipped))
        return false;

      return AppliesToVersion(game);
    }

    // ReSharper disable once InconsistentNaming

    protected abstract bool AppliesToVersion(Game game);

    protected static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk)
      => (character as CharacterObject)?.GetPerkValue(perk) ?? false;

    public override void Reset() {
    }

  }

}