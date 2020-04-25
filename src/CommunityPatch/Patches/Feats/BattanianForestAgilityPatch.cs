using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {
    public sealed class BattanianForestAgilityPatch : PatchBase<BattanianForestAgilityPatch> {
        public override bool Applied { get; protected set; }
        
        private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(BattanianForestAgilityPatch), nameof(Postfix));

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
            
            TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);

            if (faceTerrainType == TerrainType.Forest &&
                mobileParty.Leader != null &&
                mobileParty.Leader.GetFeatValue(DefaultFeats.Cultural.BattanianForestAgility)) {

                var explainedNumber = new ExplainedNumber(__result, explanation, null);

                var movingAtForestEffectField = AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "MovingAtForestEffect");
                var movingAtForestEffect = (float) movingAtForestEffectField.GetValue(__instance);

                float battanianAgilityBonus =
                    AgilityPatchShared.GetEffectBonus(DefaultFeats.Cultural.BattanianForestAgility)
                    * Math.Abs(movingAtForestEffect)
                    * baseSpeed;

                explainedNumber.Add(battanianAgilityBonus, DefaultFeats.Cultural.BattanianForestAgility.Name);

                __result = explainedNumber.ResultNumber;
            }
        }
    }
}