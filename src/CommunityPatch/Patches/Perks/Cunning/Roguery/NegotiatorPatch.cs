using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class NegotiatorPatch : PerkPatchBase<NegotiatorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultBribeCalculationModel).GetMethod(nameof(DefaultBribeCalculationModel.GetBribeToEnterLordsHall), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(NegotiatorPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x7C, 0x2F, 0x71, 0x0A, 0x54, 0x2E, 0x4F, 0x9D,
        0x5E, 0xF6, 0x5B, 0x99, 0xC8, 0x04, 0x2C, 0xE5,
        0x68, 0x6F, 0x32, 0xAB, 0x37, 0x07, 0x40, 0x56,
        0x28, 0x32, 0xAE, 0xC6, 0xC6, 0x90, 0x48, 0x8C
      }
    };

    public NegotiatorPatch() : base("lH8ZMjEo") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref int __result) {
      var scout = Hero.MainHero?.PartyBelongedTo?.EffectiveScout;
      if (scout == null) return;

      var perk = ActivePatch.Perk;
      if (!scout.GetPerkValue(perk)) return;

      __result = (int) (__result * Math.Abs(perk.PrimaryBonus));
    }

  }

}