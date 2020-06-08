using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public class MapFactionLogicLoopPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo
      // using assembly qualified name here
      // ReSharper disable once PossibleNullReferenceException
      = typeof(Hero).GetMethod("get_MapFaction",
        Public | NonPublic | Instance | Static | DeclaredOnly);

    private static readonly MethodInfo TargetMethodInfo2
      // using assembly qualified name here
      // ReSharper disable once PossibleNullReferenceException
      = typeof(PartyBase).GetMethod("get_MapFaction",
        Public | NonPublic | Instance | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo
      = typeof(MapFactionLogicLoopPatch)
        .GetMethod(nameof(Prefix),
          Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2
      = typeof(MapFactionLogicLoopPatch)
        .GetMethod(nameof(Prefix2),
          Public | NonPublic | Static | DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return TargetMethodInfo2;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.227640
        0x5D, 0xF0, 0xDC, 0xC6, 0x76, 0xB8, 0x1A, 0x28,
        0x55, 0xC3, 0x63, 0x81, 0x27, 0xD7, 0x53, 0x78,
        0x7F, 0xD1, 0x05, 0x42, 0x2D, 0x90, 0xFD, 0x2D,
        0xBB, 0x52, 0x62, 0xCC, 0x74, 0x76, 0x17, 0x8C
      },
      new byte[] {
        // e1.2.0.225830
        0x92, 0x25, 0xA9, 0xE4, 0x53, 0x37, 0x22, 0xB0,
        0x1D, 0x5F, 0x48, 0x39, 0xD3, 0x76, 0x4B, 0x09,
        0x96, 0x7D, 0xBE, 0xF2, 0xA8, 0xA9, 0x52, 0x12,
        0xB5, 0x42, 0x4C, 0x09, 0x0A, 0x99, 0x65, 0xB9
      },
      new byte[] {
        // e1.1.0.225190
        0x9A, 0x64, 0xA5, 0x6A, 0x31, 0x14, 0x78, 0x61,
        0xE1, 0x00, 0x16, 0x74, 0x97, 0xD7, 0x4C, 0x16,
        0xB7, 0xFC, 0x44, 0x4A, 0x14, 0x7B, 0x72, 0x70,
        0x4C, 0xA6, 0x99, 0x11, 0x61, 0x85, 0xB0, 0x03
      },
      new byte[] {
        // e1.4.2.231233
        0x84, 0xA8, 0xB5, 0xB1, 0xD2, 0xF5, 0xFE, 0x2D,
        0xFD, 0x8A, 0x4D, 0xC3, 0xA7, 0x9C, 0xFB, 0xCD,
        0x6F, 0x0E, 0x4A, 0x13, 0x0E, 0xAB, 0x65, 0xCA,
        0x43, 0xC1, 0xA1, 0x4B, 0x2E, 0x6D, 0x4E, 0xCA
      }
    };

    public bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatched(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo2,
        new HarmonyMethod(PatchMethodInfo2));
      Applied = true;
    }

    public static bool Prefix(Hero __instance, ref IFaction __result) {
      if (__instance.CharacterObject != null
        && (__instance.IsNotable
          || __instance.CurrentSettlement?.Party?.Owner != __instance))
        return true;

      __result = __instance.Clan?.Kingdom ?? (IFaction) __instance?.Clan ?? CampaignData.NeutralFaction;
      return false;
    }

    public static bool Prefix2(PartyBase __instance, ref IFaction __result) {
      var hero = __instance.Owner;
      if (hero == null)
        return true;
      if (hero.CharacterObject != null
        && (hero.IsNotable
          || hero.CurrentSettlement?.Party?.Owner != hero))
        return true;

      __result = hero.Clan?.Kingdom ?? (IFaction) hero?.Clan ?? CampaignData.NeutralFaction;
      return false;
    }

    public void Reset() {
    }

  }

}