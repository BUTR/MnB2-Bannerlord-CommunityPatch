using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class AgriculturePatch : PatchBase<AgriculturePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultVillageProductionCalculatorModel).GetMethod("CalculateDailyFoodProductionAmount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgriculturePatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xB6, 0x97, 0x7F, 0x48, 0x9D, 0x7D, 0x8D, 0x7F,
        0x17, 0x9B, 0x01, 0xB8, 0xC3, 0x6D, 0x87, 0x1B,
        0x9E, 0xDC, 0xF4, 0xDA, 0x8B, 0xC2, 0xFD, 0x5C,
        0xEE, 0x7C, 0x51, 0x93, 0x1D, 0x4E, 0x93, 0x5F
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "ebiXdm5W");

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
    private static void Postfix(ref float __result, Village village) {
      var perk = ActivePatch._perk;
      if (!(village.Bound?.OwnerClan?.Leader?.GetPerkValue(perk) ?? false))
        return;

      __result *= perk.PrimaryBonus;
    }

  }

}