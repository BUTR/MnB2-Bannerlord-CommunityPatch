using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class StewardNourishSettlementPatch : PatchBase<StewardNourishSettlementPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementProsperityModel).GetMethod("CalculateProsperityChangeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(StewardNourishSettlementPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(StewardNourishSettlementPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xD0, 0xC6, 0xC7, 0xCA, 0x12, 0x69, 0x13, 0x9E,
        0x0C, 0xB1, 0x5B, 0xF9, 0xB1, 0xDD, 0x3A, 0x2E,
        0x9A, 0xEF, 0xDC, 0x56, 0x5D, 0x8E, 0x62, 0x68,
        0x49, 0x9D, 0xD0, 0x2B, 0xA5, 0xE0, 0xEA, 0x39
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "KZHpJqtt");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo1),
        new HarmonyMethod(PatchMethodInfo2));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedParameter.Local
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(Town fortification, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref float __result, Town fortification, StatExplainer explanation) {
      var perk = ActivePatch._perk;
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