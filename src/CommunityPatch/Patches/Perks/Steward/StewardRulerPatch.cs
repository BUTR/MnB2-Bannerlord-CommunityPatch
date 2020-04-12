using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class StewardRulerPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Clan).GetMethod("get_CompanionLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardRulerPatch).GetMethod(nameof(CompanionLimitPatched), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x73, 0x09, 0x3A, 0xD3, 0xA9, 0x61, 0x4F, 0x54,
        0x83, 0x1B, 0x9D, 0x5E, 0x80, 0x1D, 0x53, 0xD0,
        0x2E, 0x6D, 0xEC, 0x3B, 0x69, 0x99, 0x13, 0x7D,
        0x48, 0x17, 0xFE, 0x4F, 0xAA, 0xBC, 0x7D, 0x1C
      }
    };

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        null,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CompanionLimitPatched(Clan __instance, ref int __result) {
      if (__instance.Leader.GetPerkValue(DefaultPerks.Steward.Ruler))
        __result += Town.All.Count(t => t.Owner.Owner == __instance.Leader);
    }

    public void Reset() {
    }

  }

}