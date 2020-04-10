using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

    private PerkObject _perk;

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

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xF8, 0x71, 0xC3, 0x48, 0x4B, 0xCD, 0x84, 0x83,
        0x1B, 0x74, 0xDA, 0x78, 0x22, 0x92, 0x0F, 0xE7,
        0x00, 0xE0, 0x9B, 0xE2, 0x24, 0xA1, 0x34, 0xB2,
        0x93, 0xB3, 0x22, 0x87, 0x4E, 0xB4, 0x29, 0x24
      });
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedParameter.Local
    private static void Prefix(Town fortification, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref float __result, Town fortification, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      var hero = fortification.Settlement?.OwnerClan?.Leader;

      if (hero == null || !hero.GetPerkValue(perk)
        || fortification.Settlement.Parties.Count(x => x.LeaderHero == fortification.Settlement.OwnerClan.Leader) <= 0)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      
      if (explanation.Lines.Count > 0)
        explanation.Lines.RemoveAt(explanation.Lines.Count - 1);
      
      float extra = explanation.Lines.Where(line => line.Number > 0).Sum(line => line.Number);

      if (extra < float.Epsilon)
        return;

      explainedNumber.Add(extra * perk.PrimaryBonus - extra, perk.Name);
      __result = explainedNumber.ResultNumber;
    }

  }

}