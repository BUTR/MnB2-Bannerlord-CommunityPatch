using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace CommunityPatch.Patches.Feats {
    public sealed class SturgianSnowAgilityPatch : PatchBase<SturgianSnowAgilityPatch> {
        public override bool Applied { get; protected set; }
        
        private static readonly MethodInfo PatchMethodInfo =
            AccessTools.Method(typeof(SturgianSnowAgilityPatch), nameof(Postfix));

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return AgilityPatchShared.CalculateFinalSpeedMethodInfo;
        }
        
        public override bool? IsApplicable(Game game) {
            // Currently ignores if method patched by others, expecting that there is a postfix already applied
            var hash = AgilityPatchShared.CalculateFinalSpeedMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(AgilityPatchShared.CalculateFinalSpeedHashes);
        }

        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(AgilityPatchShared.CalculateFinalSpeedMethodInfo,
                postfix: new HarmonyMethod(PatchMethodInfo));

            Applied = true;
        }

        public override void Reset() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
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
            if (atmosphereModel.SnowInfo.Density > 0f) {
                var explainedNumber = new ExplainedNumber(__result, explanation, null);

                var movingOnSnowEffectField =
                    AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "MovingOnSnowEffect");
                var movingOnSnowEffect = (float) movingOnSnowEffectField.GetValue(__instance);
                var snowDescriptionField = AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "_snow");
                var snowDescription = (TextObject) snowDescriptionField.GetValue(__instance);
                
                // if there is snow on the ground, apply the movement debuff as a factor of the density
                float snowDensityDebuff =
                    movingOnSnowEffect
                    * atmosphereModel.SnowInfo.Density
                    * baseSpeed;
                explainedNumber.Add(snowDensityDebuff, snowDescription);

                // Apply bonus to Sturgian party leaders
                if (mobileParty.Leader != null &&
                    mobileParty.Leader.GetFeatValue(DefaultFeats.Cultural.SturgianSnowAgility)) {

                    float sturgianBonus =
                        AgilityPatchShared.GetEffectBonus(DefaultFeats.Cultural.SturgianSnowAgility)
                        * Math.Abs(snowDensityDebuff);

                    explainedNumber.Add(sturgianBonus, DefaultFeats.Cultural.SturgianSnowAgility.Name);
                }

                __result = explainedNumber.ResultNumber;
            }
        }
    }
}