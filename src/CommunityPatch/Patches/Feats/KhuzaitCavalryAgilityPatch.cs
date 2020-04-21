using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Feats {
    public sealed class KhuzaitCavalryAgilityPatch : PatchBase<KhuzaitCavalryAgilityPatch> {
        public override bool Applied { get; protected set; }
        
        // private static readonly MethodInfo PureSpeedMethodInfo = typeof(DefaultPartySpeedCalculatingModel).GetMethod("CalculatePureSpeed", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo PureSpeedMethodInfo = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculatePureSpeed");
        // private static readonly MethodInfo PatchMethodInfo = typeof(KhuzaitCavalryAgilityPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(KhuzaitCavalryAgilityPatch), nameof(Postfix));

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return PureSpeedMethodInfo;
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
            // Currently ignores if method patched by others, expecting that there is a postfix applied
            return true;
        }

        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(PureSpeedMethodInfo,
                postfix: new HarmonyMethod(PatchMethodInfo));

            Applied = true;
        }

        public override void Reset() { }

        [MethodImpl(MethodImplOptions.NoInlining)]

        // public override float CalculatePureSpeed(MobileParty mobileParty, StatExplainer explanation, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0)
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
                var GetCavalryRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetCavalryRatioModifier");
                var GetMountedFootmenRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetMountedFootmenRatioModifier");

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

                int min_footmenCount_numberOfAvailableMounts = Math.Min(footmenCount, numberOfAvailableMounts);

                float cavalryRatioModifier = (float) GetCavalryRatioModifierMethod.Invoke(__instance, new object[] {menCount, horsemenCount});
                float mountedFootmenRatioModifier = (float) GetMountedFootmenRatioModifierMethod.Invoke(__instance, new object[] {menCount, min_footmenCount_numberOfAvailableMounts});

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