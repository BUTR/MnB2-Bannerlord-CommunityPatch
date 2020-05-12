using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class SlipIntoShadowsPatch : PerkPatchBase<SlipIntoShadowsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultDisguiseDetectionModel).GetMethod(nameof(DefaultDisguiseDetectionModel.CalculateDisguiseDetectionProbability),
      Public | NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SlipIntoShadowsPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xAD, 0x2A, 0x34, 0x12, 0x4D, 0x03, 0xF5, 0x3D,
        0x12, 0xE9, 0x1C, 0xFE, 0x07, 0x49, 0xFB, 0x75,
        0xB2, 0x55, 0xC1, 0xA9, 0xF0, 0xB7, 0xF9, 0xBB,
        0x1F, 0x00, 0xF2, 0x7F, 0x04, 0x77, 0x84, 0x8E
      },
      new byte[] {
        // e1.4.0.228869
        0x1A, 0x65, 0x84, 0xA7, 0x10, 0x17, 0xDB, 0x7F,
        0xE2, 0x09, 0x72, 0xE5, 0xDB, 0xB5, 0x6B, 0xC7,
        0x93, 0x14, 0xA3, 0x40, 0xDD, 0x78, 0xE1, 0xB9,
        0xCF, 0x9B, 0xAB, 0xE9, 0x53, 0xAD, 0x6B, 0xD3
      }
    };

    public SlipIntoShadowsPatch() : base("Eth2Z6qK") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (TargetMethodInfo == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref float __result) {
      var perk = ActivePatch.Perk;
      var scout = Hero.MainHero.PartyBelongedTo.EffectiveScout;
      if (scout?.GetPerkValue(perk) != true) return;

      __result -= perk.PrimaryBonus;
    }

  }

}