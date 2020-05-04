using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public class BrokenClanLeadersPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo
      // using assembly qualified name here
      // ReSharper disable once PossibleNullReferenceException
      = typeof(Clan).GetMethod("get_Leader",
        Public | NonPublic | Instance | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo
      = typeof(BrokenClanLeadersPatch)
        .GetMethod(nameof(Postfix),
          Public | NonPublic | Static | DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatched(patchInfo))
        return false;

      //var hash = TargetMethodInfo.MakeCilSignatureSha256();
      //return hash.MatchesAnySha256(Hashes);
      return true;
    }

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    
    public static void Postfix(Clan __instance, ref Hero __result) {
      if (__result == null)
        return;

      if (__result.Clan != null)
        return;

      try {
        __result.Clan = __instance;
      }
      catch {
        // hero w/o party, gg custom spawns
      }
    }

    public void Reset() {
    }

  }

}