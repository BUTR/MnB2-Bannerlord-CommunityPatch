using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class AgriculturePatch : PatchBase<AgriculturePatch> {
    
    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultVillageProductionCalculatorModel).GetMethod("CalculateDailyFoodProductionAmount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(AgriculturePatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }
    
    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.8
        0xBE, 0xB9, 0x2E, 0x39, 0x09, 0xCF, 0x91, 0x9B,
        0x23, 0x00, 0x33, 0xA2, 0xB2, 0x9D, 0xDF, 0xF6,
        0x8F, 0xD0, 0xC5, 0x59, 0x3E, 0xB1, 0xE6, 0xB7,
        0xCE, 0x54, 0xAB, 0x4C, 0xFD, 0x16, 0xB0, 0x57
      },
      new byte[] {
        // e1.1.0
        0x34, 0x2D, 0xF6, 0x0B, 0x44, 0x5C, 0xFB, 0xB5,
        0x4B, 0xDA, 0x13, 0x6C, 0x6C, 0xCC, 0xFE, 0x9D,
        0xDB, 0x4C, 0xA1, 0xF3, 0x15, 0xAE, 0x0C, 0xAB,
        0xB4, 0x1A, 0x14, 0x87, 0x78, 0x33, 0x16, 0x71
      },
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