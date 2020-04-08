using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class ManAtArmsExplainerPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(PartyBase).GetMethod("get_PartySizeLimitExplainer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ManAtArmsExplainerPatch).GetMethod(nameof(PartySizeLimitExplainerPatched), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

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

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x02, 0x3C, 0xD3, 0xE3, 0xF6, 0x5C, 0xC3, 0x74,
        0x3C, 0x53, 0xA5, 0x9E, 0x17, 0x54, 0x6E, 0x87,
        0x6A, 0xF3, 0xFE, 0xF8, 0xDF, 0x1A, 0x55, 0xD8,
        0x1F, 0x83, 0xBB, 0x7A, 0x13, 0xB6, 0x97, 0xFC
      });
    }

    private static void PartySizeLimitExplainerPatched(PartyBase __instance, ref StatExplainer __result) {
      var extra = ManAtArmsPatch.ManAtArmsPerkExtra(__instance.LeaderHero);

      if (extra > 0)
        __result.AddLine("Man-At-Arms", extra);
    }
    
    public void Reset() {}

  }

}