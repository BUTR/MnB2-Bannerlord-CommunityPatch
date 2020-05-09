using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch.Patches.Feats {

  public sealed class AseraiCheapCaravansPatch : PatchBase<AseraiCheapCaravansPatch> {

    public override bool Applied { get; protected set; }

    private static readonly Type TargetType = Type.GetType("SandBox.LordConversationsCampaignBehavior, SandBox, Version=1.0.0.0, Culture=neutral");

    private static readonly MethodInfo TargetMethodInfo = AccessTools.Method(TargetType, "conversation_magistrate_form_a_caravan_accept_on_consequence");

    private static readonly MethodInfo PrefixPatchMethodInfo = AccessTools.Method(typeof(AseraiCheapCaravansPatch), nameof(Prefix));

    private static readonly MethodInfo PostfixPatchMethodInfo = AccessTools.Method(typeof(AseraiCheapCaravansPatch), nameof(Postfix));

    public static readonly byte[][] TargetHashes = {
      new byte[] {
        // e1.2.0.226271
        0x72, 0x71, 0xCF, 0x96, 0x32, 0xA5, 0x98, 0x86,
        0xAB, 0x61, 0xDD, 0x80, 0x8E, 0x36, 0xCC, 0xF2,
        0x77, 0x37, 0x51, 0xC3, 0x5B, 0x6A, 0x46, 0xF6,
        0x22, 0x1B, 0x88, 0xED, 0x36, 0xFB, 0xAA, 0x7A
      },
      new byte[] {
        // e1.3.0.227640
        0x04, 0x9A, 0x8B, 0xF9, 0x6B, 0x3E, 0x7F, 0x41,
        0x71, 0x68, 0x1A, 0x12, 0xF2, 0xC5, 0x08, 0x9D,
        0x96, 0xF0, 0x1A, 0x85, 0xC2, 0x7D, 0x95, 0xEB,
        0xED, 0xA6, 0x63, 0x7A, 0x98, 0x90, 0xA6, 0xC1
      },
      new byte[] {
        // e1.4.0.228531
        0x92, 0x68, 0x8E, 0x68, 0x38, 0x3D, 0xC7, 0x5F,
        0x0B, 0x7F, 0x49, 0xD9, 0xD8, 0x7B, 0xD7, 0x31,
        0x6A, 0x39, 0x34, 0xFD, 0x28, 0x16, 0x59, 0xC0,
        0xB9, 0x71, 0xB0, 0xE9, 0xCC, 0x83, 0x85, 0x2F
      },
      new byte[] {
        // e1.4.0.228616
        0xC7, 0xCD, 0x5F, 0x01, 0xBF, 0xEC, 0x52, 0x5F,
        0x50, 0xFC, 0xE9, 0xBC, 0xA9, 0x02, 0xB7, 0x6F,
        0x07, 0xAB, 0xA1, 0x0B, 0xC0, 0xD5, 0x7D, 0xFD,
        0x80, 0xC1, 0x99, 0x51, 0x98, 0x8B, 0xF9, 0x1D
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

    public override void Reset() {
    }

    static void Prefix(out int __state)
      // get player gold and make it available for Postfix
    {
      var hero = (ConversationSentence.LastSelectedRepeatObject as CharacterObject)?.HeroObject;
      __state = hero?.Gold ?? 0;
    }

    static void Postfix(int __state) {
      var hero = (ConversationSentence.LastSelectedRepeatObject as CharacterObject)?.HeroObject;
      if (hero == null)
        return;

      // presume the player successfully purchased a caravan if their gold is less than it was at the beginning
      // of the target function call
      if (hero.Gold >= __state
        || !hero.IsHumanPlayerCharacter
        || !hero.CharacterObject.GetFeatValue(DefaultFeats.Cultural.AseraiCheapCaravans))
        return;

      var creditAmount = (int) (DefaultFeats.Cultural.AseraiCheapCaravans.EffectBonus * (__state - hero.Gold));

      if (creditAmount <= 0)
        return;

      if (hero == Hero.MainHero)
        InformationManager.DisplayMessage(new InformationMessage(AseraiCheapCaravansFlavorText.ToString()));

      // Apply credit
      if (hero.CurrentSettlement != null)
        GiveGoldAction.ApplyForSettlementToCharacter(hero.CurrentSettlement, hero, creditAmount);
    }

  }

}