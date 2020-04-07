using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class StewardRulerPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Clan).GetMethod("get_CompanionLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardRulerPatch).GetMethod(nameof(CompanionLimitPatched), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        null,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;
      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

     var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xC8, 0xA5, 0xFD, 0x25, 0xE8, 0x42, 0x2F, 0x6E,
        0x5F, 0xC6, 0x02, 0xBB, 0x06, 0x2C, 0x6A, 0x9E,
        0x61, 0xD8, 0x48, 0x36, 0x2D, 0xA8, 0x5C, 0x20,
        0x2D, 0xEF, 0x2B, 0x64, 0x87, 0x59, 0x4B, 0x8D
      });
    }

    private static void CompanionLimitPatched(Clan __instance, ref int __result) {
      if (__instance.Leader.GetPerkValue(DefaultPerks.Steward.Ruler))
        __result += Town.All.Count(t => t.Owner.Owner == __instance.Leader);
    }

  }

}