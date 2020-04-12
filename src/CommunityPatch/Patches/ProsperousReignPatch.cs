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

  sealed class ProsperousReignPatch : PatchBase<ProsperousReignPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementProsperityModel).GetMethod("CalculateHearthChangeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(ProsperousReignPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(ProsperousReignPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x93, 0x92, 0xD6, 0xB0, 0x05, 0x07, 0xEF, 0xFA,
        0xFA, 0x86, 0x1D, 0xC4, 0x3A, 0xA3, 0x78, 0x07,
        0x27, 0x33, 0x4A, 0x6A, 0xCC, 0xB7, 0x9F, 0xFF,
        0x49, 0x3A, 0xE8, 0x50, 0x71, 0x54, 0x89, 0x0A
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "5MjmCaUx");

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

    // ReSharper disable once UnusedParameter.Local
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Prefix(Village village, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(ref float __result, Village village, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(village.Bound?.OwnerClan?.Leader?.GetPerkValue(perk) ?? false))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);

      if (!(__result > -0.0001f) || !(__result < 0.0001f) && explanation.Lines.Count > 0)
        explanation.Lines.RemoveAt(explanation.Lines.Count - 1);

      var extra = explanation.Lines.Where(line => line.Number > 0).Sum(line => line.Number);

      if (extra < float.Epsilon)
        return;

      explainedNumber.Add(extra * perk.PrimaryBonus - extra, perk.Name);
      __result = explainedNumber.ResultNumber;
    }

  }

}