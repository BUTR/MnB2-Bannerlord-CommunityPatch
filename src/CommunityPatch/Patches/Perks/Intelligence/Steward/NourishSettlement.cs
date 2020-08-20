using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class NourishSettlementPatch : PerkPatchBase<NourishSettlementPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementProsperityModel).GetMethod("CalculateProsperityChangeInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(NourishSettlementPatch).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(NourishSettlementPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xBA, 0x7A, 0x87, 0x92, 0x7D, 0x27, 0x63, 0x03,
        0xC3, 0x32, 0xA5, 0x3A, 0x46, 0x36, 0x01, 0xB6,
        0x44, 0x52, 0x67, 0xBB, 0x41, 0x76, 0xD0, 0xAC,
        0x02, 0x53, 0xA2, 0x67, 0xA4, 0x42, 0x5A, 0xBF
      },
      new byte[] {
        // e1.3.0.227640
        0xCF, 0xC0, 0x5B, 0x18, 0x3D, 0x79, 0xC0, 0xC9,
        0x15, 0xD2, 0x33, 0x49, 0xEB, 0x5E, 0xF3, 0x00,
        0x4C, 0xB1, 0x2A, 0x8C, 0x48, 0x9C, 0x05, 0x40,
        0xF0, 0xC2, 0xF5, 0x53, 0x42, 0x9C, 0x2B, 0x49
      },
      new byte[] {
        // e1.4.0.228531
        0x3A, 0x7F, 0x90, 0xD8, 0x7A, 0x08, 0xC4, 0x0A,
        0x00, 0xC8, 0x88, 0xE4, 0x57, 0x6D, 0xAE, 0x77,
        0x26, 0x28, 0x39, 0x30, 0x4E, 0x85, 0x88, 0x2E,
        0x8A, 0x09, 0x2E, 0x43, 0x2D, 0xBD, 0x4D, 0x23
      },
      new byte[] {
        // e1.4.1.230527
        0x0C, 0x2C, 0x68, 0xC9, 0xA1, 0x12, 0x26, 0x2F,
        0x83, 0x48, 0x01, 0x1F, 0x6A, 0xB5, 0xBA, 0x3E,
        0xE8, 0x58, 0xE9, 0x9B, 0xF0, 0x48, 0x96, 0x0A,
        0x84, 0x2B, 0xD0, 0x11, 0xC9, 0xC1, 0x5B, 0x27
      },
      new byte[] {
        // e1.4.2.231233
        0xF8, 0x6D, 0x40, 0xEE, 0xBA, 0x4C, 0xED, 0x2F,
        0xA8, 0x3B, 0x56, 0x17, 0xB4, 0x00, 0x65, 0xF3,
        0x1E, 0xB0, 0x70, 0x1F, 0x7E, 0x8E, 0x87, 0x19,
        0x19, 0x11, 0xA9, 0xFC, 0xFD, 0x63, 0x9E, 0x8C
      },
      new byte[] {
        // e1.4.3.237794
        0x7A, 0xAB, 0x35, 0xBC, 0xF9, 0x74, 0x56, 0xE0,
        0x2C, 0x26, 0xD8, 0xED, 0xB5, 0x2F, 0xAD, 0x67,
        0x01, 0xE4, 0xB0, 0xEC, 0xBC, 0x51, 0xAF, 0x79,
        0x3C, 0x68, 0xC8, 0x3A, 0xE9, 0x22, 0xD5, 0x3C
      }
    };

    public NourishSettlementPatch() : base("KZHpJqtt") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo1),
        new HarmonyMethod(PatchMethodInfo2));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedParameter.Local

    private static void Prefix(Town fortification, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref float __result, Town fortification, StatExplainer explanation) {
      var perk = ActivePatch.Perk;
      var hero = fortification.Settlement?.OwnerClan?.Leader;

      if (hero == null || !hero.GetPerkValue(perk)
        || fortification.Settlement.Parties.Count(x => x.LeaderHero == fortification.Settlement.OwnerClan.Leader) <= 0)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);

      if (explanation.Lines.Count > 0)
        explanation.Lines.RemoveAt(explanation.Lines.Count - 1);

      var extra = explanation.Lines.Where(line => line.Number > 0).Sum(line => line.Number);

      if (extra < float.Epsilon)
        return;

      explainedNumber.Add(extra * perk.PrimaryBonus - extra, perk.Name);
      __result = explainedNumber.ResultNumber;
    }

  }

}