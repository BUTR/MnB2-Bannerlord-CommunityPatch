using System.Reflection;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch {

  public static class PatchApplicabilityHelper {

    public static bool IsTargetPatchable(MethodInfo targetMethodInfo, byte[][] targetHashes) {
      var patchInfo = Harmony.GetPatchInfo(targetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = targetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(targetHashes);
    }

  }

}