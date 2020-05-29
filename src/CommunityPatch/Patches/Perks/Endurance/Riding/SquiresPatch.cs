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

  public sealed class SquiresPatch : PerkPatchBase<SquiresPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SquiresPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

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
        // e1.4.1.229326
        0x2A, 0x25, 0x16, 0xFA, 0x67, 0xCB, 0x5E, 0xBB,
        0x9E, 0x32, 0x1E, 0x19, 0x72, 0xBB, 0x42, 0xE1,
        0xEC, 0x1D, 0x23, 0xA6, 0x4E, 0x2B, 0xAA, 0x36,
        0x1B, 0xC6, 0x64, 0x00, 0x01, 0x75, 0x90, 0xB1
      },
      new byte[] {
        // e1.4.1.230527
        0xE7, 0x8E, 0x12, 0xAF, 0xDA, 0x4B, 0x20, 0x6D,
        0x5A, 0x69, 0x5F, 0x02, 0xCD, 0xE3, 0xA5, 0x6A,
        0x43, 0x23, 0x27, 0xC2, 0x33, 0x16, 0xB4, 0x14,
        0xF1, 0x96, 0xC0, 0x53, 0x64, 0x31, 0x94, 0x15
      }
    };

    public SquiresPatch() : base("qaAKXRSV") {
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

      var extra = perk.PrimaryBonus * party.MemberRoster.Count(x => x.Character.IsHero && x.Character.HeroObject.GetPerkValue(perk));
      if (extra <= 0)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      var baseLine = explanation?.Lines.Find(x => x.Name == "Base");
      if (baseLine != null)
        explanation.Lines.Remove(baseLine);

      explainedNumber.Add(extra, perk.Name);
      __result = (int) explainedNumber.ResultNumber;
    }

  }

}