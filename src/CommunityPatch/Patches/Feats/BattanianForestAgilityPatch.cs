using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {
    public class BattanianForestAgilityPatch : PatchBase<BattanianForestAgilityPatch> {
        public override bool Applied { get; protected set; }
        
        private static readonly MethodInfo FinalSpeedMethodInfo = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed");
        private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(BattanianForestAgilityPatch), nameof(Postfix));

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return FinalSpeedMethodInfo;
        }
        
        private static readonly byte[][] ValidHashes = {
            new byte[] {
                // e1.1.0.224785
                0x58, 0xF5, 0x64, 0xA2, 0xE1, 0x17, 0x5C, 0x0C,
                0x13, 0xEE, 0xED, 0xA8, 0x5E, 0xFC, 0xDF, 0x38,
                0x7E, 0xCE, 0x1E, 0xF9, 0xF8, 0x67, 0x78, 0x3F,
                0xB9, 0x71, 0xE0, 0x02, 0x8D, 0x58, 0x40, 0x99
            }
        };
        
        public override bool? IsApplicable(Game game) {
            // Currently ignores if method patched by others, expecting that there is a postfix already applied
            var hash = FinalSpeedMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(ValidHashes);
        }
        
        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(FinalSpeedMethodInfo,
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

                var explainedNumber = new ExplainedNumber(baseSpeed, explanation, null);

                var movingAtForestEffectField = AccessTools.Field(typeof(DefaultPartySpeedCalculatingModel), "MovingAtForestEffect");
                var movingAtForestEffect = (float) movingAtForestEffectField.GetValue(__instance);

                var battanianAgilityBonus =
                    DefaultFeats.Cultural.BattanianForestAgility.EffectBonus * Math.Abs(movingAtForestEffect);

                explainedNumber.AddFactor(battanianAgilityBonus, DefaultFeats.Cultural.BattanianForestAgility.Name);

                __result = explainedNumber.ResultNumber;
            }
        }
    }
}