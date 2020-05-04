using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
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
    
public ResolutePatch() : base("aNEj0uIa") {}

private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x43, 0x16, 0x41, 0xFC, 0xDA, 0xF5, 0x69, 0xBE,
        0xFB, 0x81, 0xB6, 0x66, 0xD9, 0x15, 0x5D, 0xE9,
        0xB3, 0xC0, 0xD2, 0x1C, 0x93, 0xC2, 0x70, 0x50,
        0x24, 0xDB, 0x47, 0xC6, 0xEE, 0x7C, 0xD6, 0x19
      }
    };
    
    public override bool? IsApplicable(Game game)
    {
      if (Perk == null) return false;
      
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
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
    
    private static float GetPartyMemberBonus(MobileParty mobileParty, PerkObject perk)
    {
      var perkPartyMemberValue = new ExplainedNumber(100);
      PerkHelper.AddPerkBonusForParty(perk, mobileParty, ref perkPartyMemberValue);
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

    private static int CalculateTotalMoraleLossReduction(DefaultPartyMoraleModel model, MobileParty party, float reductionRate)
    {
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
      => GetMoralePenaltyFromMethod(model, "GetNoWageMoralePenalty", mobileParty) * mobileParty.HasUnpaidWages;

    private static float CalculateStarvationMoralePenalty(DefaultPartyMoraleModel model, MobileParty mobileParty)
      => mobileParty.Party.IsStarving ? GetMoralePenaltyFromMethod(model, "GetStarvationMoralePenalty", mobileParty) : 0f;

    private static float CalculatePartySizeMoralePenalty(DefaultPartyMoraleModel model, MobileParty party)
      => GetMoralePenaltyFromMethodWithExplainedNumber(model, "GetPartySizeMoraleEffect", party);

    private static float CalculateFoodVarietyPenalty(DefaultPartyMoraleModel model, MobileParty party)
      => GetMoralePenaltyFromMethodWithExplainedNumber(model, "CalculateFoodVarietyMoraleBonus", party);

    private static float GetMoralePenaltyFromMethod(DefaultPartyMoraleModel model, string methodName, MobileParty party) {
      var method = DefaultPartyMoraleModelType.GetMethod(methodName, Instance | DeclaredOnly | NonPublic);
      if (method == null) return 0f;
      
      var moraleBonus = (int) method.Invoke(model, new object[] {party});
      return GetMoralePenalty(moraleBonus);
    }
    
    private static float GetMoralePenaltyFromMethodWithExplainedNumber(DefaultPartyMoraleModel model, string methodName, MobileParty party) {
      var method = DefaultPartyMoraleModelType.GetMethod(methodName, Instance | NonPublic | DeclaredOnly);
      if (method == null) return 0f;
      
      var args = new object[] { party, new ExplainedNumber(0f) };
      method.Invoke(model, args);
      var moraleBonus = (ExplainedNumber) args[1];
      return GetMoralePenalty(moraleBonus.ResultNumber);
    }
    
    private static int GetMoralePenalty(int moraleBonus)
      => moraleBonus >= 0 ? 0 : moraleBonus;
    private static float GetMoralePenalty(float moraleBonus)
      => moraleBonus >= 0f ? 0f : moraleBonus;
    
  }
}