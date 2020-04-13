using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
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

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartyWageModel).GetMethod("GetTotalWage", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPrefix =
      typeof(CharterOfLibertiesPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix =
      typeof(CharterOfLibertiesPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // GetTotalWage: e1.0.10
        0x36, 0x5F, 0x8C, 0x6F, 0x66, 0x0D, 0xD9, 0x92,
        0xEF, 0x11, 0xDE, 0x81, 0x5A, 0xF6, 0x4C, 0xA7,
        0x5E, 0x17, 0x21, 0x36, 0x94, 0x8C, 0x6C, 0x95,
        0xA3, 0x1F, 0xE9, 0xF6, 0xC0, 0xFC, 0x67, 0x6E
      }
    };

    public override void Reset() {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfoPrefix),
        new HarmonyMethod(PatchMethodInfoPostfix));

      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo1 = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo1))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref int __result, MobileParty mobileParty, StatExplainer explanation) {
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

  }

}