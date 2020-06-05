using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch {

  public static class PatchApplicabilityHelper {

    private static readonly ApplicationVersionComparer VersionComparer = CommunityPatchSubModule.VersionComparer;
    private static readonly ApplicationVersion GameVersion = CommunityPatchSubModule.GameVersion;
    
    public static String SkippedPatchReason(IPatch patch) {
      var reason = "Skipped";
      if (IsPatchObsolete(patch))
        reason = "Obsolete";
      if (!IsPatchForGameVersion(patch))
        reason = "Non compatible";
      return reason;
    }
    
    public static bool IsPatchObsolete(IPatch patch) {
      var obsolete = patch.GetType().GetCustomAttribute<PatchObsoleteAttribute>();
      return obsolete != null && VersionComparer.Compare(GameVersion, obsolete.Version) >= 0;
    }
    
    public static bool IsPatchForGameVersion(IPatch patch) {
      var notBefore = patch.GetType().GetCustomAttribute<PatchNotBeforeAttribute>();
      return !(notBefore != null && VersionComparer.Compare(GameVersion, notBefore.Version) < 0);
    }
    
    public static bool IsTargetPatchable(MethodInfo targetMethodInfo, byte[][] targetHashes) {
      if (targetMethodInfo == null)
        return false;

      var patchInfo = Harmony.GetPatchInfo(targetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = targetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(targetHashes);
    }

  }

}