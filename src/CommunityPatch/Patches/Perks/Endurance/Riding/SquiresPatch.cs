using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class SquiresPatch : PatchBase<SquiresPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SquiresPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

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
        // e1.0.11
        0x75, 0x7C, 0x73, 0x06, 0xD6, 0xBB, 0xF3, 0xFC,
        0xA6, 0x65, 0xEF, 0x79, 0xDA, 0x11, 0x04, 0x75,
        0x23, 0x28, 0xBD, 0x4E, 0xC5, 0x95, 0x0F, 0x5E,
        0x71, 0xD6, 0x8C, 0x75, 0xC4, 0xDF, 0x52, 0x5F
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "qaAKXRSV");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch._perk;

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