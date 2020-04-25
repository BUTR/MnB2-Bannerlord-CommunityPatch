using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class SlipIntoShadowsPatch : PatchBase<SlipIntoShadowsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultDisguiseDetectionModel).GetMethod(nameof(DefaultDisguiseDetectionModel.CalculateDisguiseDetectionProbability),
      Public | NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(SlipIntoShadowsPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xAD, 0x2A, 0x34, 0x12, 0x4D, 0x03, 0xF5, 0x3D,
        0x12, 0xE9, 0x1C, 0xFE, 0x07, 0x49, 0xFB, 0x75,
        0xB2, 0x55, 0xC1, 0xA9, 0xF0, 0xB7, 0xF9, 0xBB,
        0x1F, 0x00, 0xF2, 0x7F, 0x04, 0x77, 0x84, 0x8E
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "Eth2Z6qK");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      if (TargetMethodInfo == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref float __result) {
      var perk = ActivePatch._perk;
      var scout = Hero.MainHero.PartyBelongedTo.EffectiveScout;
      if (scout?.GetPerkValue(perk) != true) return;

      __result -= perk.PrimaryBonus;
    }

  }

}