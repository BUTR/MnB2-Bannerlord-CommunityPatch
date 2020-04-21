using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {
    public sealed class KhuzaitCavalryAgilityPatch : PatchBase<KhuzaitCavalryAgilityPatch> {
        public override bool Applied { get; protected set; }
        
        private static readonly MethodInfo PureSpeedMethodInfo = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculatePureSpeed");
        private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(KhuzaitCavalryAgilityPatch), nameof(Postfix));

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return PureSpeedMethodInfo;
        }

        private static readonly byte[][] ValidHashes = {
            new byte[] {
                // e1.2.0.226271 and presumably previous versions
                0x61, 0xD7, 0x4D, 0xF5, 0xB0, 0x0E, 0x84, 0x52,
                0xDC, 0xCB, 0x2F, 0xE7, 0xE2, 0x20, 0x38, 0x10,
                0x87, 0x01, 0xE3, 0x61, 0xF1, 0xAB, 0x89, 0x7D,
                0x9C, 0xDC, 0x50, 0x6E, 0xA6, 0x7E, 0xEB, 0xEF
            }
        };
        
        public override bool? IsApplicable(Game game) {
            // Currently ignores if method patched by others, expecting that there is a postfix already applied
            var hash = PureSpeedMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(ValidHashes);
        }

        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(PureSpeedMethodInfo,
                postfix: new HarmonyMethod(PatchMethodInfo));

            Applied = true;
        }

        public override void Reset() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Postfix(
            DefaultPartySpeedCalculatingModel __instance,
            ref MobileParty mobileParty,
            ref StatExplainer explanation,
            ref int additionalTroopOnFootCount,
            ref int additionalTroopOnHorseCount,
            ref float __result) {

            // Only calculate if party leader is Khuzait (should apply to NPC lords)
            if (mobileParty.Leader != null &&
                mobileParty.Leader.GetFeatValue(DefaultFeats.Cultural.KhuzaitCavalryAgility)) {

                // Get private methods needed for calculation
                var getCavalryRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetCavalryRatioModifier");
                var getMountedFootmenRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetMountedFootmenRatioModifier");

                // Recalculate Cavalry and Footmen on horses speed modifiers based on 
                int menCount = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount +
                               additionalTroopOnHorseCount;
                int horsemenCount = mobileParty.Party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
                int footmenCount = mobileParty.Party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
                int numberOfAvailableMounts =
                    mobileParty.ItemRoster.NumberOfMounts; // Do this instead of calling AddCargoStats()

                if (mobileParty.AttachedParties.Count != 0) {
                    foreach (var attachedParty in mobileParty.AttachedParties) {
                        menCount += attachedParty.MemberRoster.TotalManCount;
                        horsemenCount += attachedParty.Party.NumberOfMenWithHorse;
                        footmenCount += attachedParty.Party.NumberOfMenWithoutHorse;
                        numberOfAvailableMounts +=
                            attachedParty.ItemRoster.NumberOfMounts; // Do this instead of calling AddCargoStats()
                    }
                }

                int minFootmenCountNumberOfAvailableMounts = Math.Min(footmenCount, numberOfAvailableMounts);

                float cavalryRatioModifier = (float) getCavalryRatioModifierMethod.Invoke(__instance, new object[] {menCount, horsemenCount});
                float mountedFootmenRatioModifier = (float) getMountedFootmenRatioModifierMethod.Invoke(__instance, new object[] {menCount, minFootmenCountNumberOfAvailableMounts});

                // calculate Khuzait bonus and apply
                float khuzaitRatioModifier = DefaultFeats.Cultural.KhuzaitCavalryAgility.EffectBonus *
                                             (cavalryRatioModifier + mountedFootmenRatioModifier);
                var explainedNumber = new ExplainedNumber(__result, explanation, null);

                explainedNumber.AddFactor(khuzaitRatioModifier, DefaultFeats.Cultural.KhuzaitCavalryAgility.Name);
                __result = explainedNumber.ResultNumber;
            }
        }
    }
}