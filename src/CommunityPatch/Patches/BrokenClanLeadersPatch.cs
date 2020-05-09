using System.Collections.Generic;
using System.Reflection;
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

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.227640
        0x19, 0xA8, 0x52, 0x06, 0xF6, 0x89, 0xE3, 0xEB,
        0xC5, 0x91, 0x32, 0x64, 0x2B, 0x61, 0xC0, 0xAF,
        0xD5, 0x27, 0x7A, 0x91, 0x89, 0xF2, 0x16, 0x8D,
        0xB7, 0x68, 0xB9, 0xC3, 0x4B, 0xA8, 0x4F, 0xE3
      }
    };

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatched(patchInfo))
        return false;

      return TargetMethodInfo.MakeCilSignatureSha256()
        .MatchesAnySha256(Hashes);
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