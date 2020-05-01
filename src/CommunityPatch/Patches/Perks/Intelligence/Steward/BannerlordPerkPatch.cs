using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class BannerlordPatch : PatchBase<BannerlordPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(BannerlordPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225664
        0x41, 0xDD, 0x60, 0x12, 0x52, 0xAC, 0x6C, 0xA7,
        0x8A, 0xB4, 0x95, 0x75, 0xA5, 0x51, 0x90, 0x95,
        0x66, 0x1D, 0x76, 0x3F, 0xB1, 0xC5, 0x73, 0x74,
        0xDE, 0x3B, 0xD3, 0xB2, 0xF6, 0x30, 0x4E, 0xC5
      },
      new byte[] {
        // e1.1.0.224785
        0x75, 0x7C, 0x73, 0x06, 0xD6, 0xBB, 0xF3, 0xFC,
        0xA6, 0x65, 0xEF, 0x79, 0xDA, 0x11, 0x04, 0x75,
        0x23, 0x28, 0xBD, 0x4E, 0xC5, 0x95, 0x0F, 0x5E,
        0x71, 0xD6, 0x8C, 0x75, 0xC4, 0xDF, 0x52, 0x5F
      },
      new byte[] {
        // e1.0.5
        0x5A, 0xFE, 0xC3, 0xB1, 0x6A, 0xFC, 0xE0, 0xE0,
        0x43, 0x83, 0x5B, 0x73, 0x2F, 0x7D, 0x29, 0x9F,
        0x63, 0xAE, 0xD2, 0xC1, 0x6B, 0xE0, 0x0F, 0x32,
        0x38, 0x4E, 0x81, 0x18, 0xE2, 0xF3, 0x61, 0x18
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "MMv0U5Yr");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
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
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(party.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      var fiefCount = party.LeaderHero.Clan.Settlements.Count(s => !s.IsVillage);
      var extra = fiefCount * perk.PrimaryBonus;

      if (extra < float.Epsilon)
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