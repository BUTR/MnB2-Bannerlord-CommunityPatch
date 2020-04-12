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
        // e1.1.0.224785
        0xA6, 0xA0, 0xF8, 0x4C, 0x5B, 0xF7, 0xF9, 0x8D,
        0xDA, 0x45, 0x65, 0x5E, 0x19, 0x62, 0xA6, 0x13,
        0x5E, 0x45, 0x3C, 0xE5, 0x19, 0xFD, 0x71, 0x4B,
        0xF3, 0xF8, 0xCA, 0xA8, 0xFD, 0xD7, 0x0F, 0xEF
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