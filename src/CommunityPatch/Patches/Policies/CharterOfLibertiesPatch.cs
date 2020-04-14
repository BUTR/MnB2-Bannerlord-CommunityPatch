using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Policies {

  /* Done:
   * - Revenue paid to both liege and local ruler reduced by 5%
   */

  /* TODO:
   * - Militia quantity is increased by 10%
   * - 10% of militia troops will be higher tier
   */

  public sealed class CharterOfLibertiesPatch : PatchBase<CharterOfLibertiesPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo1 =
      typeof(DefaultPartyWageModel).GetMethod("GetTotalWage", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo TargetMethodInfo2 =
      typeof(MilitiasCampaignBehavior).GetMethod("SpawnMilitiaParty", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix1 =
      typeof(CharterOfLibertiesPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix1 =
      typeof(CharterOfLibertiesPatch).GetMethod(nameof(Postfix1), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix2 =
      typeof(CharterOfLibertiesPatch).GetMethod(nameof(Postfix2), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo1;
      yield return TargetMethodInfo2;
    }

    private static readonly byte[][] Hashes1 = {
      new byte[] {
        // GetTotalWage: e1.0.10
        0x36, 0x5F, 0x8C, 0x6F, 0x66, 0x0D, 0xD9, 0x92,
        0xEF, 0x11, 0xDE, 0x81, 0x5A, 0xF6, 0x4C, 0xA7,
        0x5E, 0x17, 0x21, 0x36, 0x94, 0x8C, 0x6C, 0x95,
        0xA3, 0x1F, 0xE9, 0xF6, 0xC0, 0xFC, 0x67, 0x6E
      }
    };

    private static readonly byte[][] Hashes2 = {
      new byte[] {
        // SpawnMilitiaParty: e1.0.11
        0x8C, 0x6A, 0x50, 0x4F, 0x6F, 0xA2, 0x40, 0xC8,
        0xAF, 0x1D, 0xCC, 0x09, 0xC7, 0x50, 0xA0, 0x00,
        0x15, 0x67, 0xE0, 0x24, 0x36, 0x1B, 0xDE, 0xD9,
        0x1B, 0x65, 0x71, 0xFF, 0x84, 0xDF, 0xEF, 0xD4
      }
    };

    public override void Reset() {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo1,
        new HarmonyMethod(PatchMethodInfoPrefix1),
        new HarmonyMethod(PatchMethodInfoPostfix1));

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo2,
        postfix: new HarmonyMethod(PatchMethodInfoPostfix2));

      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo1 = Harmony.GetPatchInfo(TargetMethodInfo1);
      if (AlreadyPatchedByOthers(patchInfo1))
        return false;

      var hash1 = TargetMethodInfo1.MakeCilSignatureSha256();
      if (!hash1.MatchesAnySha256(Hashes1))
        return false;

      var patchInfo2 = Harmony.GetPatchInfo(TargetMethodInfo2);
      if (AlreadyPatchedByOthers(patchInfo2))
        return false;

      var hash2 = TargetMethodInfo2.MakeCilSignatureSha256();
      return hash2.MatchesAnySha256(Hashes2);
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix1(ref int __result, MobileParty mobileParty, StatExplainer explanation) {
      if (!mobileParty.IsMilitia)
        return;

      var partyLeader = mobileParty.LeaderHero;
      var kingdom = partyLeader?.Clan?.Kingdom;

      if (kingdom == null || !kingdom.ActivePolicies.Contains(DefaultPolicies.CharterOfLiberties))
        return;

      if (mobileParty.HomeSettlement.OwnerClan.Leader != partyLeader || mobileParty.HomeSettlement.OwnerClan.Leader != kingdom.Ruler)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      explainedNumber.AddFactor(-0.05f, DefaultPolicies.CharterOfLiberties.Name);

      __result = (int) explainedNumber.ResultNumber;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix2(Settlement settlement, int militiaCount) {
      if (militiaCount == 0)
        return;

      var party = settlement.MilitaParty;
      if (party == null)
        return;

      var roster = party.MemberRoster;
      if (roster.Count == 0)
        return;

      // Upgrade and add troops
    }

  }

}