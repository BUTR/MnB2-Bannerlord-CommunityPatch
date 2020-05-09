using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Feats {

  public sealed class SturgianSnowAgilityPatch : PatchBase<SturgianSnowAgilityPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo PatchMethodInfo =
      AccessTools.Method(typeof(SturgianSnowAgilityPatch), nameof(Postfix));

    private static readonly TextObject SnowDescription = AccessTools.StaticFieldRefAccess<DefaultPartySpeedCalculatingModel, TextObject>("_snow");

    private static readonly float MovingOnSnowEffect = (float) AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "MovingOnSnowEffect").GetRawConstantValue();

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
      // Check if on snowy terrain
      var atmosphereModel =
        Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, mobileParty.GetPosition());

      // SnowInfo.Density is between 0.0f and 1.0f
      if (atmosphereModel.SnowInfo.Density < float.Epsilon)
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);

      // if there is snow on the ground, apply the movement debuff as a factor of the density
      var snowDensityDebuff =
        MovingOnSnowEffect
        * atmosphereModel.SnowInfo.Density
        * baseSpeed;
      explainedNumber.Add(snowDensityDebuff, SnowDescription);

      // Apply bonus to Sturgian party leaders
      var feat = DefaultFeats.Cultural.SturgianSnowAgility;

      if (mobileParty.Leader != null &&
        mobileParty.Leader.GetFeatValue(feat)) {
        var sturgianBonus =
          AgilityPatchShared.GetEffectBonus(feat)
          * Math.Abs(snowDensityDebuff);

        explainedNumber.Add(sturgianBonus, feat.Name);
      }

      __result = explainedNumber.ResultNumber;
    }

  }

}