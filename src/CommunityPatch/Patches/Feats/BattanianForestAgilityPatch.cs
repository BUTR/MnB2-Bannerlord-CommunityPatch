using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {

  public sealed class BattanianForestAgilityPatch : PatchBase<BattanianForestAgilityPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(BattanianForestAgilityPatch), nameof(Postfix));

    private static readonly float MovingAtForestEffect = (float) AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "MovingAtForestEffect").GetRawConstantValue();

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AgilityPatchShared.CalculateFinalSpeedMethodInfo;
    }

    public static byte[][] CalculateFinalSpeedHashes => AgilityPatchShared.CalculateFinalSpeedHashes;

    public override bool? IsApplicable(Game game) {
      // Currently ignores if method patched by others, expecting that there is a postfix already applied
      var hash = AgilityPatchShared.CalculateFinalSpeedMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(CalculateFinalSpeedHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(AgilityPatchShared.CalculateFinalSpeedMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    public override void Reset() {
    }

    private static void Postfix(
      DefaultPartySpeedCalculatingModel __instance,
      ref MobileParty mobileParty,
      ref float baseSpeed,
      ref StatExplainer explanation,
      ref float __result) {
      var faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);

      var feat = DefaultFeats.Cultural.BattanianForestAgility;

      if (faceTerrainType != TerrainType.Forest
        || mobileParty.Leader == null
        || !mobileParty.Leader.GetFeatValue(feat))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);

      var battanianAgilityBonus =
        AgilityPatchShared.GetEffectBonus(feat)
        * Math.Abs(MovingAtForestEffect)
        * baseSpeed;

      explainedNumber.Add(battanianAgilityBonus, feat.Name);

      __result = explainedNumber.ResultNumber;
    }

  }

}