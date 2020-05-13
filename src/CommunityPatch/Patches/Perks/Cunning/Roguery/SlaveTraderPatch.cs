using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class SlaveTraderPatch : PerkPatchBase<SlaveTraderPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(CharacterObject).GetMethod(nameof(CharacterObject.PrisonerRansomValue), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SlaveTraderPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xB4, 0xEA, 0x03, 0x63, 0x1F, 0x2B, 0xA6, 0x8C,
        0x4A, 0x2E, 0x86, 0xFE, 0x67, 0xCD, 0x82, 0x4D,
        0x79, 0x6B, 0xA4, 0xDC, 0x38, 0xC3, 0xCE, 0xE7,
        0x8A, 0x98, 0x20, 0x9D, 0xC2, 0x2E, 0x1E, 0x88
      }
    };

    public SlaveTraderPatch() : base("jNbTBxEW") {
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
      Perk.SetPrimaryBonus(.20f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref int __result, Hero sellerHero = null) {
      if (sellerHero == null) return;

      var perk = ActivePatch.Perk;
      if (!sellerHero.GetPerkValue(perk)) return;

      __result += (int) (__result * perk.PrimaryBonus);
    }

  }

}