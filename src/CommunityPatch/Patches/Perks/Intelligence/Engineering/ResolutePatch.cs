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
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {
  public class ResolutePatch : PatchBase<ResolutePatch> {
    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartyMoraleModel).GetMethod(nameof(DefaultPartyMoraleModel.GetEffectivePartyMorale), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ResolutePatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }
    
    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "aNEj0uIa");

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x43, 0x16, 0x41, 0xFC, 0xDA, 0xF5, 0x69, 0xBE,
        0xFB, 0x81, 0xB6, 0x66, 0xD9, 0x15, 0x5D, 0xE9,
        0xB3, 0xC0, 0xD2, 0x1C, 0x93, 0xC2, 0x70, 0x50,
        0x24, 0xDB, 0x47, 0xC6, 0xEE, 0x7C, 0xD6, 0x19
      }
    };
    
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public override bool? IsApplicable(Game game)
    {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;
      
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }
    
    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );
      
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 50f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref float __result, MobileParty mobileParty, StatExplainer explanation = null) {
      var perk = ActivePatch._perk;
      
      if (mobileParty.SiegeEvent == null) return;
      var partyMemberBonus = GetPartyMemberBonus(mobileParty, perk);
      if (partyMemberBonus < .01) return;

      var partyMembersCount = (int) (partyMemberBonus / perk.PrimaryBonus);
      var multiplicativeBonus = CalculateMultiplicativeBonus(perk.PrimaryBonus / 100, partyMembersCount);
      var moraleLossReductionRate = multiplicativeBonus * -1;
      
      var totalMoraleLossReduction = CalculateTotalMoraleLossReduction(mobileParty, moraleLossReductionRate);
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

    private static int CalculateTotalMoraleLossReduction(MobileParty mobileParty, float moraleLossReductionRate)
    {
      var recentEventsMoraleLossReduction = Math.Min(mobileParty.RecentEventsMorale, 0) * moraleLossReductionRate;
      var starvationMoraleLossReduction = mobileParty.Party.IsStarving ? -30 * moraleLossReductionRate : 0;
      var unpaidWageMoraleLossReduction = mobileParty.HasUnpaidWages > 0f ? -20 * moraleLossReductionRate : 0;
      var partySizeMoraleLossReduction = CalculatePartySizeMoralePenalty(mobileParty) * moraleLossReductionRate;
      var foodVarietyMoraleLossReduction = CalculateFoodVarietyPenalty(mobileParty) * moraleLossReductionRate;

      var totalMoraleLossReduction = (int) (recentEventsMoraleLossReduction +
        starvationMoraleLossReduction +
        unpaidWageMoraleLossReduction +
        partySizeMoraleLossReduction +
        foodVarietyMoraleLossReduction);
      
      return totalMoraleLossReduction;
    }
    private static float CalculatePartySizeMoralePenalty(MobileParty party) {
      var oversize = party.Party.NumberOfAllMembers - party.Party.PartySizeLimit;
      if (oversize > 0) return -1f * (float) Math.Sqrt(oversize);
      return 0f;
    }

    private static float CalculateFoodVarietyPenalty(MobileParty party) {
      var variety = party.ItemRoster.FoodVariety;
      if (variety <= 1) return -2;
      if (variety <= 2) return -1;
      return 0;
    }
  }
}