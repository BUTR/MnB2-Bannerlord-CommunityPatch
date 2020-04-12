using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class LogisticsExpertPatch : PatchBase<LogisticsExpertPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculateFinalSpeed", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(LogisticsExpertPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x58, 0xF5, 0x64, 0xA2, 0xE1, 0x17, 0x5C, 0x0C,
        0x13, 0xEE, 0xED, 0xA8, 0x5E, 0xFC, 0xDF, 0x38,
        0x7E, 0xCE, 0x1E, 0xF9, 0xF8, 0x67, 0x78, 0x3F,
        0xB9, 0x71, 0xE0, 0x02, 0x8D, 0x58, 0x40, 0x99
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "6JaeM2p2");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
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
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref float __result, MobileParty mobileParty, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(mobileParty.Army?.LeaderParty?.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);

      __result = explainedNumber.ResultNumber;
    }

  }

}