using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class PrisonerRecruitmentPatch : PatchBase<PrisonerRecruitmentPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPrisonerRecruitmentCalculationModel).GetMethod(nameof(DefaultPrisonerRecruitmentCalculationModel.GetDailyRecruitedPrisoners), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo LikelyPatchedOfficialMethodInfo =
      typeof(RecruitPrisonersCampaignBehavior).GetMethod("DailyTick", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo PatchMethodInfo = typeof(PrisonerRecruitmentPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override void Reset() {
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var targetBytes = TargetMethodInfo.GetCilBytes();
      var officialPatchBytes = LikelyPatchedOfficialMethodInfo.GetCilBytes();
      if (targetBytes == null || officialPatchBytes == null) return false;

      var targetHash = targetBytes.GetSha256();
      var officialPatchHash = officialPatchBytes.GetSha256();
      return targetHash.SequenceEqual(new byte[] {
        0x54, 0xF4, 0xBA, 0x36, 0x27, 0x51, 0xF2, 0x72,
        0x81, 0x8F, 0x14, 0xE3, 0x33, 0xA2, 0x52, 0x63,
        0xD6, 0x16, 0xAF, 0x91, 0x19, 0x8F, 0xE8, 0xA5,
        0xF8, 0x23, 0xDF, 0x7D, 0xEF, 0x80, 0x2D, 0xBC
      })
        && officialPatchHash.SequenceEqual(new byte[] {
          0xA2, 0xD2, 0x2D, 0xD1, 0x88, 0x35, 0x70, 0x41,
          0xF6, 0x1D, 0x31, 0x07, 0xF9, 0x65, 0x1C, 0x01,
          0xED, 0x44, 0xDC, 0x92, 0xE9, 0x3D, 0x71, 0x31,
          0x5D, 0x42, 0x41, 0xBA, 0x27, 0x0B, 0x0C, 0x70
        });
    }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref float[] __result) {
      var newResult = new float[__result.Length+1];
        newResult[0] = 1f;
        Array.Copy(__result, 0, newResult, 1, __result.Length);
        __result = newResult;
    }
  }
}