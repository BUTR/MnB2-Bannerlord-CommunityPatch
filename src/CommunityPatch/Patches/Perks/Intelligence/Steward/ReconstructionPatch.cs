using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Actions;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class ReconstructionPatch : PerkPatchBase<ReconstructionPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(IncreaseSettlementHealthAction).GetMethod("ApplyInternal", NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ReconstructionPatch).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.10
        0x28, 0xDC, 0x69, 0xF2, 0xC3, 0xC2, 0x19, 0xBD,
        0x70, 0x9C, 0x02, 0xD6, 0x76, 0x9D, 0x73, 0xA1,
        0xF1, 0x01, 0x4F, 0x52, 0x81, 0x3D, 0x93, 0x3C,
        0xCB, 0x70, 0x31, 0x89, 0xF8, 0xD1, 0x5D, 0x2D
      }
    };

    public ReconstructionPatch() : base("Fa01e9kY") {
    }

    public override void Apply(Game game) {
      Perk.Modify(2.0f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    // ReSharper disable once InconsistentNaming

    private static void Prefix(Settlement settlement, ref float percentage) {
      var perk = ActivePatch.Perk;
      var governor = settlement.Town?.Governor;
      if (governor == null || !governor.GetPerkValue(perk))
        return;

      percentage *= perk.PrimaryBonus;
    }

  }

}