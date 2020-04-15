using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public class MapFactionLogicLoopPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo
      // using assembly qualified name here
      // ReSharper disable once PossibleNullReferenceException
      = typeof(Hero).GetMethod("get_MapFaction",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo
      = typeof(MapFactionLogicLoopPatch)
        .GetMethod(nameof(Prefix),
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x9A, 0x64, 0xA5, 0x6A, 0x31, 0x14, 0x78, 0x61,
        0xE1, 0x00, 0x16, 0x74, 0x97, 0xD7, 0x4C, 0x16,
        0xB7, 0xFC, 0x44, 0x4A, 0x14, 0x7B, 0x72, 0x70,
        0x4C, 0xA6, 0x99, 0x11, 0x61, 0x85, 0xB0, 0x03
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
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Prefix(Hero __instance, ref IFaction __result) {
      if (__instance.IsNotable
        || __instance.CurrentSettlement?.Party?.Owner != __instance)
        return true;

      __result = __instance?.Clan?.Kingdom ?? (IFaction) __instance?.Clan ?? CampaignData.NeutralFaction;
      return false;
    }

    public void Reset() {
    }

  }

}