using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class ResolutePatch : PerkPatchBase<ResolutePatch> {

    public override bool Applied { get; protected set; }

    private static readonly Type DefaultPartyMoraleModelType = typeof(DefaultPartyMoraleModel);

    private static readonly MethodInfo TargetMethodInfo = DefaultPartyMoraleModelType.GetMethod(nameof(DefaultPartyMoraleModel.GetEffectivePartyMorale), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ResolutePatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public ResolutePatch() : base("aNEj0uIa") {
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x43, 0x16, 0x41, 0xFC, 0xDA, 0xF5, 0x69, 0xBE,
        0xFB, 0x81, 0xB6, 0x66, 0xD9, 0x15, 0x5D, 0xE9,
        0xB3, 0xC0, 0xD2, 0x1C, 0x93, 0xC2, 0x70, 0x50,
        0x24, 0xDB, 0x47, 0xC6, 0xEE, 0x7C, 0xD6, 0x19
      },
      new byte[] {
        // e1.4.1.230527
        0x3C, 0xBD, 0xD3, 0x1A, 0xEC, 0x38, 0x53, 0x96,
        0x4F, 0xFE, 0x6A, 0x1E, 0xE2, 0x7A, 0x60, 0xE8,
        0xC4, 0x1A, 0x97, 0x01, 0x88, 0x31, 0xE6, 0x92,
        0x9C, 0x89, 0x0D, 0x3F, 0x89, 0x62, 0x6A, 0xBA
      },
      new byte[] {
        // e1.5.1.241359
        0x1C, 0x8D, 0xC8, 0xCA, 0x5B, 0x8C, 0x30, 0xAC,
        0xE4, 0xF9, 0x56, 0x42, 0x7D, 0x73, 0x0E, 0x75,
        0x28, 0xA3, 0x8E, 0xA3, 0x06, 0xE0, 0x4A, 0x3F,
        0x5F, 0xDD, 0xB7, 0xD3, 0x73, 0x59, 0x84, 0x32
      }
    };

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(50f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    public static void Postfix(ref float __result, ref DefaultPartyMoraleModel __instance, MobileParty mobileParty, StatExplainer explanation = null) {
      var perk = ActivePatch.Perk;

      if (mobileParty.SiegeEvent == null) return;

      var partyMemberBonus = GetPartyMemberBonus(mobileParty, perk);
      if (partyMemberBonus.IsEqualOrLesserThan(0f)) return;

      var partyMembersCount = (int) (partyMemberBonus / perk.PrimaryBonus);
      var multiplicativeBonus = CalculateMultiplicativeBonus(perk.PrimaryBonus / 100, partyMembersCount);
      var moraleLossReductionRate = multiplicativeBonus * -1;

      var totalMoraleLossReduction = CalculateTotalMoraleLossReduction(__instance, mobileParty, moraleLossReductionRate);
      explanation?.AddLine(perk.Name.ToString(), totalMoraleLossReduction);
      __result += totalMoraleLossReduction;
    }

    private static float GetPartyMemberBonus(MobileParty mobileParty, PerkObject perk) {
      var perkPartyMemberValue = new ExplainedNumber(100);
#if AFTER_E1_5_1
      PerkHelper.AddPerkBonusForParty(perk, mobileParty, true, ref perkPartyMemberValue);
#else
      PerkHelper.AddPerkBonusForParty(perk, mobileParty, ref perkPartyMemberValue);
#endif 
      var additiveMoralePenaltyReductionRate = perkPartyMemberValue.ResultNumber - perkPartyMemberValue.BaseNumber;
      return additiveMoralePenaltyReductionRate;
    }

    private static float CalculateMultiplicativeBonus(float partyMemberBonus, int partyMemberCount) {
      var finalRate = 0f;
      var lastRate = 1f;

      for (var i = 0; i < partyMemberCount; i++) {
        var nextRate = partyMemberBonus * lastRate;
        finalRate += nextRate;
        lastRate = nextRate;
      }

      return finalRate;
    }

    private static int CalculateTotalMoraleLossReduction(DefaultPartyMoraleModel model, MobileParty party, float reductionRate) {
      var recentEventsMoraleLossReduction = Math.Min(party.RecentEventsMorale, 0);
      var starvationMoraleLossReduction = CalculateStarvationMoralePenalty(model, party);
      var unpaidWageMoraleLossReduction = CalculateUnpaidWageMoralePenalty(model, party);
      var partySizeMoraleLossReduction = CalculatePartySizeMoralePenalty(model, party);
      var foodVarietyMoraleLossReduction = CalculateFoodVarietyPenalty(model, party);

      var totalMoraleLossReduction = recentEventsMoraleLossReduction +
        starvationMoraleLossReduction +
        unpaidWageMoraleLossReduction +
        partySizeMoraleLossReduction +
        foodVarietyMoraleLossReduction;

      return (int) (totalMoraleLossReduction * reductionRate);
    }

    private static float CalculateUnpaidWageMoralePenalty(DefaultPartyMoraleModel model, MobileParty mobileParty)
      => GetMoralePenalty(GetNoWageMoralePenalty(model, mobileParty)) * mobileParty.HasUnpaidWages;

    private static float CalculateStarvationMoralePenalty(DefaultPartyMoraleModel model, MobileParty mobileParty)
      => mobileParty.Party.IsStarving ? GetMoralePenalty(GetStarvationMoralePenalty(model, mobileParty)) : 0f;

    private static float CalculatePartySizeMoralePenalty(DefaultPartyMoraleModel model, MobileParty party) {
      var number = new ExplainedNumber(0f);
      GetPartySizeMoraleEffect(model, party, ref number);
      return GetMoralePenalty(number.ResultNumber);
    }

    private static float CalculateFoodVarietyPenalty(DefaultPartyMoraleModel model, MobileParty party) {
      var number = new ExplainedNumber(0f);
      CalculateFoodVarietyMoraleBonus(model, party, ref number);
      return GetMoralePenalty(number.ResultNumber);
    }

    private static readonly Func<DefaultPartyMoraleModel, MobileParty, int> GetNoWageMoralePenalty =
      DefaultPartyMoraleModelType.GetMethod(nameof(GetNoWageMoralePenalty), Instance | DeclaredOnly | NonPublic).BuildInvoker<Func<DefaultPartyMoraleModel, MobileParty, int>>();

    private static readonly Func<DefaultPartyMoraleModel, MobileParty, int> GetStarvationMoralePenalty =
      DefaultPartyMoraleModelType.GetMethod(nameof(GetStarvationMoralePenalty), Instance | DeclaredOnly | NonPublic).BuildInvoker<Func<DefaultPartyMoraleModel, MobileParty, int>>();

    private delegate void GetMoralePenaltyFromMethodWithExplainedNumberDelegate(DefaultPartyMoraleModel model, MobileParty party, ref ExplainedNumber explainer);

    private static readonly GetMoralePenaltyFromMethodWithExplainedNumberDelegate GetPartySizeMoraleEffect =
      DefaultPartyMoraleModelType.GetMethod(nameof(GetPartySizeMoraleEffect), Instance | DeclaredOnly | NonPublic).BuildInvoker<GetMoralePenaltyFromMethodWithExplainedNumberDelegate>();

    private static readonly GetMoralePenaltyFromMethodWithExplainedNumberDelegate CalculateFoodVarietyMoraleBonus =
      DefaultPartyMoraleModelType.GetMethod(nameof(CalculateFoodVarietyMoraleBonus), Instance | DeclaredOnly | NonPublic).BuildInvoker<GetMoralePenaltyFromMethodWithExplainedNumberDelegate>();

    private static int GetMoralePenalty(int moraleBonus)
      => moraleBonus >= 0 ? 0 : moraleBonus;

    private static float GetMoralePenalty(float moraleBonus)
      => moraleBonus >= 0f ? 0f : moraleBonus;

  }

}