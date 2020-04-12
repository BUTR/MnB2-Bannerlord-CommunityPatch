using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class WarmongerPatch : PatchBase<WarmongerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultArmyManagementCalculationModel).GetMethod("CalculatePartyInfluenceCost", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarmongerPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e.1.0.9
        0x89, 0x81, 0x49, 0x0B, 0x2D, 0x71, 0x5D, 0x56,
        0xAE, 0x3C, 0x96, 0x5A, 0xF4, 0x9D, 0xBC, 0x9D,
        0xE3, 0x0E, 0x59, 0xFD, 0x57, 0x85, 0x2B, 0xA4,
        0x5E, 0xFD, 0x63, 0xE2, 0x9A, 0x5D, 0xAE, 0x31
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "ldk9Xvod");

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
    private static void Postfix(ref int __result, MobileParty armyLeaderParty) {
      var perk = ActivePatch._perk;
      if (!(armyLeaderParty.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      __result = (int) Math.Round(__result * (1 + perk.PrimaryBonus));
    }

  }

}