using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class ConroiPatch : PerkPatchBase<ConroiPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ConroiPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225664
        0x41, 0xDD, 0x60, 0x12, 0x52, 0xAC, 0x6C, 0xA7,
        0x8A, 0xB4, 0x95, 0x75, 0xA5, 0x51, 0x90, 0x95,
        0x66, 0x1D, 0x76, 0x3F, 0xB1, 0xC5, 0x73, 0x74,
        0xDE, 0x3B, 0xD3, 0xB2, 0xF6, 0x30, 0x4E, 0xC5
      },
      new byte[] {
        // e1.0.11
        0x75, 0x7C, 0x73, 0x06, 0xD6, 0xBB, 0xF3, 0xFC,
        0xA6, 0x65, 0xEF, 0x79, 0xDA, 0x11, 0x04, 0x75,
        0x23, 0x28, 0xBD, 0x4E, 0xC5, 0x95, 0x0F, 0x5E,
        0x71, 0xD6, 0x8C, 0x75, 0xC4, 0xDF, 0x52, 0x5F
      },
      new byte[] {
        // e1.4.1.231234
        0xF2, 0x6B, 0x63, 0xD5, 0xF6, 0xE5, 0xD0, 0xEE,
        0xED, 0xF8, 0x57, 0xC5, 0xC0, 0x12, 0x1B, 0x68,
        0x29, 0xC1, 0x68, 0xAA, 0x30, 0x3C, 0x59, 0x3B,
        0x54, 0x83, 0x46, 0x3B, 0x04, 0x74, 0x92, 0x82
      },
      new byte[] {
        // e1.4.2.231233
        0x2D, 0x94, 0xDA, 0x75, 0xDD, 0xBB, 0x61, 0x7B,
        0xB2, 0x2A, 0x35, 0xDA, 0x37, 0x6D, 0x93, 0x2A,
        0x05, 0x7D, 0xF5, 0x18, 0x03, 0x52, 0xD2, 0x95,
        0x8E, 0x7E, 0xA4, 0x10, 0x96, 0x57, 0x2C, 0x71
      }
    };

    public ConroiPatch() : base("k15fVuc4") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch.Perk;

      var extra = perk.PrimaryBonus * (party?.MemberRoster
        .Count(x
          => x.Character != null
          && x.Character.IsHero
          && x.Character.HeroObject != null
          && x.Character.HeroObject.GetPerkValue(perk)
        ) ?? 1f);
      if (extra <= 0)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      var baseLine = explanation?.Lines?.Find(x => x?.Name == "Base");
      if (baseLine != null)
        explanation.Lines.Remove(baseLine);

      explainedNumber.Add(extra, perk.Name);
      __result = (int) explainedNumber.ResultNumber;
    }

  }

}