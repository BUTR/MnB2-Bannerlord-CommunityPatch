using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Feats {
    public sealed class AseraiCheapCaravansPatch : PatchBase<AseraiCheapCaravansPatch> {
        public override bool Applied { get; protected set; }
        
        private static readonly Type TargetType = AccessTools.TypeByName("SandBox.LordConversationsCampaignBehavior");

        private static readonly MethodInfo TargetMethodInfo = AccessTools.Method(TargetType, "conversation_magistrate_form_a_caravan_accept_on_consequence");
        private static readonly MethodInfo PrefixPatchMethodInfo = AccessTools.Method(typeof(AseraiCheapCaravansPatch), nameof(Prefix));
        private static readonly MethodInfo PostfixPatchMethodInfo = AccessTools.Method(typeof(AseraiCheapCaravansPatch), nameof(Postfix));

        private static readonly byte[][] TargetHashes = {
            new byte[] {
                // e1.2.0.226271
                0x72, 0x71, 0xCF, 0x96, 0x32, 0xA5, 0x98, 0x86,
                0xAB, 0x61, 0xDD, 0x80, 0x8E, 0x36, 0xCC, 0xF2,
                0x77, 0x37, 0x51, 0xC3, 0x5B, 0x6A, 0x46, 0xF6,
                0x22, 0x1B, 0x88, 0xED, 0x36, 0xFB, 0xAA, 0x7A
            }
        };
        
        private static readonly TextObject AseraiCheapCaravansFlavorText = new TextObject(
            "{=AseraiCheapCaravansFlavorText}An Aserai clerk nods to you and slips a coin purse into your hands.");

        public override IEnumerable<MethodBase> GetMethodsChecked() {
            yield return AseraiCheapCaravansPatch.TargetMethodInfo;
        }
        
        public override bool? IsApplicable(Game game) {
            // This patch does not modify the target function with Prefix, which only provides state data for Postfix
            // It should be compatible with other patches

            var hash = TargetMethodInfo.MakeCilSignatureSha256();
            return hash.MatchesAnySha256(TargetHashes);
        }
        
        public override void Apply(Game game) {
            if (Applied) return;

            CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
                prefix: new HarmonyMethod(PrefixPatchMethodInfo),
                postfix: new HarmonyMethod(PostfixPatchMethodInfo));
                
                Applied = true;
        }
        
        public override void Reset() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Prefix(out int __state) {
            // get player gold and make it available for Postfix
            __state = Hero.MainHero.Gold;
        }

        static void Postfix(int __state) {
            // presume the player successfully purchased a caravan if their gold is less than it was at the beginning
            // of the target function call
            if (Hero.MainHero.Gold < __state &&
                Hero.MainHero.IsHumanPlayerCharacter &&
                Hero.MainHero.CharacterObject.GetFeatValue(DefaultFeats.Cultural.AseraiCheapCaravans)) {

                int creditAmount = (int) (DefaultFeats.Cultural.AseraiCheapCaravans.EffectBonus * (__state - Hero.MainHero.Gold));

                if (creditAmount > 0) {
                    InformationManager.DisplayMessage(new InformationMessage(AseraiCheapCaravansFlavorText.ToString()));

                    // Apply credit
                    GiveGoldAction.ApplyForSettlementToCharacter(Settlement.CurrentSettlement, Hero.MainHero, creditAmount, false);
                }
            }
        }
    }
}
