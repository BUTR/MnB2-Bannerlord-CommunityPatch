using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class WarmongerPatch : PerkPatchBase<WarmongerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultArmyManagementCalculationModel).GetMethod("CalculatePartyInfluenceCost", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarmongerPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x1B, 0x73, 0xA3, 0x7B, 0xB4, 0x88, 0x33, 0x47,
        0x1E, 0x26, 0x32, 0xC1, 0x40, 0x2B, 0xEB, 0x2D,
        0xE5, 0xF6, 0xB1, 0xAA, 0xAB, 0x7A, 0xC9, 0xDE,
        0x91, 0xEC, 0xBD, 0x9E, 0x2E, 0x89, 0x2B, 0x81
      },
      new byte[] {
        // e1.4.1.229326
        0x54, 0xA0, 0x16, 0x47, 0xFB, 0xA8, 0x33, 0xA8,
        0xEB, 0x3E, 0x39, 0xA6, 0x6E, 0x9A, 0x9A, 0x53,
        0x6B, 0x52, 0xB0, 0x89, 0x6A, 0x4C, 0x0D, 0x1B,
        0x2B, 0x52, 0xB0, 0xDA, 0x34, 0xCB, 0xD0, 0x0D
      }
    };

    public WarmongerPatch() : base("ldk9Xvod") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref int __result, MobileParty armyLeaderParty) {
      var perk = ActivePatch.Perk;
      if (!(armyLeaderParty.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      __result = (int) Math.Round(__result * (1 + perk.PrimaryBonus));
    }

  }

}