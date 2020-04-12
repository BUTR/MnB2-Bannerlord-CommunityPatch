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
        // e.1.0.9
        0x65, 0x10, 0x58, 0xD9, 0xC7, 0xFA, 0x99, 0x07,
        0xB5, 0x92, 0xC0, 0xB9, 0xC4, 0xEF, 0x94, 0xBF,
        0x5D, 0x7B, 0xCA, 0xA0, 0x1E, 0x37, 0xB9, 0x73,
        0xFA, 0x2C, 0x1D, 0x91, 0xC0, 0x5F, 0xF1, 0x14
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