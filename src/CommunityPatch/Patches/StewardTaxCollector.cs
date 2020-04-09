using System;
using System.Reflection;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class StewardTaxCollectorPatch : PatchBase<StewardTaxCollectorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateDailyTaxInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardTaxCollectorPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "UbxBYp60");

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;
      
      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x0A, 0xCC, 0xF6, 0x94, 0xF3, 0x0D, 0x5E, 0xC9,
        0xDC, 0x61, 0x7D, 0xAE, 0x96, 0xFC, 0x97, 0x80,
        0x11, 0xDC, 0xF9, 0x65, 0x72, 0x3B, 0x82, 0x23,
        0xC2, 0xC7, 0x15, 0x8B, 0x58, 0x35, 0x03, 0x85
      });
    }

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref int __result, Town town, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(town?.Governor?.GetPerkValue(perk) ?? false))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);
      if (explainedNumber.BaseNumber > 0) {
        explainedNumber.AddFactor(perk.PrimaryBonus, perk.Name);
        __result = (int) explainedNumber.ResultNumber;
      }
    }

  }

}