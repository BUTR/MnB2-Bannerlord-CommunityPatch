using System.Collections.Generic;
using System.Linq;
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
        0x06, 0xE6, 0xB4, 0xA5, 0xA5, 0x0C, 0xAD, 0x4E,
        0x5C, 0xCB, 0xB8, 0xBD, 0x80, 0x43, 0x52, 0x81,
        0x70, 0x70, 0x34, 0x4E, 0x14, 0xA0, 0x58, 0x93,
        0xAB, 0x0C, 0x84, 0xAC, 0x43, 0x3F, 0xDD, 0x63
      },
      new byte[] {
        // e1.1.0
        0xA3, 0xB7, 0x92, 0x6A, 0xE5, 0xFA, 0x4F, 0xE1,
        0xBF, 0x05, 0x63, 0x1D, 0xF1, 0xD5, 0x04, 0xCE,
        0x89, 0xCE, 0x40, 0x77, 0xC9, 0x98, 0x03, 0x49,
        0xC2, 0x02, 0x41, 0x7A, 0x08, 0x66, 0x78, 0x14
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