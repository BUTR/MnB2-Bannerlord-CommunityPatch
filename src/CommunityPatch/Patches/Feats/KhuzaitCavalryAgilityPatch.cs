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
        
        private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(KhuzaitCavalryAgilityPatch), nameof(Postfix));

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return AgilityPatchShared.CalculatePureSpeedMethodInfo;
        }

        public override bool? IsApplicable(Game game) {
            // Currently ignores if method patched by others, expecting that there is a postfix already applied
            var hash = AgilityPatchShared.CalculatePureSpeedMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(AgilityPatchShared.CalculatePureSpeedHashes);
        }

        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(AgilityPatchShared.CalculatePureSpeedMethodInfo,
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
                var calculateBaseSpeedForPartyMethod = AccessTools.Method(__instance.GetType(), "CalculateBaseSpeedForParty");
                var getCavalryRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetCavalryRatioModifier");
                var getMountedFootmenRatioModifierMethod = AccessTools.Method(__instance.GetType(), "GetMountedFootmenRatioModifier");

                // Recalculate Cavalry and Footmen on horses speed modifiers based on 
                var menCount = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount +
                               additionalTroopOnHorseCount;

                var baseNumber = (float) calculateBaseSpeedForPartyMethod.Invoke(__instance, new object[] {menCount});
                var horsemenCount = mobileParty.Party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
                var footmenCount = mobileParty.Party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
                var numberOfAvailableMounts =
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

                var minFootmenCountNumberOfAvailableMounts = Math.Min(footmenCount, numberOfAvailableMounts);

                var cavalryRatioModifier = (float) getCavalryRatioModifierMethod.Invoke(__instance, new object[] {menCount, horsemenCount});
                var mountedFootmenRatioModifier = (float) getMountedFootmenRatioModifierMethod.Invoke(__instance, new object[] {menCount, minFootmenCountNumberOfAvailableMounts});

                // calculate Khuzait bonus and apply
                var khuzaitBonus =
                    AgilityPatchShared.GetEffectBonus(DefaultFeats.Cultural.KhuzaitCavalryAgility)
                    * (cavalryRatioModifier + mountedFootmenRatioModifier)
                    * baseNumber;

                var explainedNumber = new ExplainedNumber(__result, explanation, null);

                explainedNumber.Add(khuzaitBonus, DefaultFeats.Cultural.KhuzaitCavalryAgility.Name);
                __result = explainedNumber.ResultNumber;
            }
        }
    }
}