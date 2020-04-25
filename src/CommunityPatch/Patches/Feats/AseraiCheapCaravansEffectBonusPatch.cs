using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {
    public sealed class AseraiCheapCaravansEffectBonusPatch : PatchBase<AseraiCheapCaravansEffectBonusPatch> {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = AccessTools.Method(typeof(DefaultFeats), "InitializeAll");
        private static readonly float PatchedEffectBonusValue = 0.30f;  // 30% per character creation screen

        private static readonly byte[][] TargetHashes = {
            new byte[] {
                // e1.2.0.226271
                0xA4, 0x93, 0xCF, 0x2E, 0x6F, 0x90, 0x38, 0x41,
                0x5C, 0x15, 0x2E, 0x81, 0x1A, 0xCA, 0x13, 0x87,
                0x37, 0xD2, 0x63, 0x5A, 0x06, 0xED, 0x31, 0x7F,
                0x7A, 0x99, 0x82, 0xBB, 0x1F, 0x07, 0xB3, 0xE1
            }
        };
        
        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return TargetMethodInfo;
        }
        
        public override bool? IsApplicable(Game game) {
            if (Applied ||
                TargetMethodInfo == null ||
                !DefaultFeats.Cultural.AseraiCheapCaravans.EffectBonus.IsDifferentFrom(PatchedEffectBonusValue)) {
                
                return false;
            }

            var hash = TargetMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(TargetHashes);
        }

        public override void Apply(Game game) {
            if (Applied) return;

            FeatObject featObject =
                (FeatObject) AccessTools.Field(
                    typeof(DefaultFeats), "_cultureAseraiCheapCaravans")?.GetValue(Campaign.Current.DefaultFeats);

            if (featObject == null) {
                Applied = false;
            }
            else {
                featObject.Initialize(
                    featObject.Name.ToString(),
                    featObject.Description.ToString(),
                    PatchedEffectBonusValue,
                    featObject.IncrementType);

                Applied = !featObject.EffectBonus.IsDifferentFrom(PatchedEffectBonusValue);

                if (!Applied) {
                    CommunityPatchSubModule.Error("AseraiCheapCaravansEffectBonusPatch:  Feat not initialized with new EffectBonus value");
                }
            }
        }

        public override void Reset()
          => Applied = false;

    }
}
