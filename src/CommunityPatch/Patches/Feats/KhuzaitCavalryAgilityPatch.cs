using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {

  public sealed class KhuzaitCavalryAgilityPatch : PatchBase<KhuzaitCavalryAgilityPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo PatchMethodInfo = AccessTools.Method(typeof(KhuzaitCavalryAgilityPatch), nameof(Postfix));

    private static readonly MethodInfo CalculateBaseSpeedForPartyMethod = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateBaseSpeedForParty");

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, float> CalculateBaseSpeedForParty = CalculateBaseSpeedForPartyMethod.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, float>>();

    private static readonly MethodInfo GetCavalryRatioModifierMethod = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "GetCavalryRatioModifier");

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, int, float> GetCavalryRatioModifier = GetCavalryRatioModifierMethod.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, int, float>>();

    private static readonly MethodInfo GetMountedFootmenRatioModifierMethod = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "GetMountedFootmenRatioModifier");

    private static readonly Func<DefaultPartySpeedCalculatingModel, int, int, float> GetMountedFootmenRatioModifier = GetMountedFootmenRatioModifierMethod.BuildInvoker<Func<DefaultPartySpeedCalculatingModel, int, int, float>>();

    public static byte[][] CalculatePureSpeedHashes => AgilityPatchShared.CalculatePureSpeedHashes;

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AgilityPatchShared.CalculatePureSpeedMethodInfo;
    }

    public override bool? IsApplicable(Game game) {
      // Currently ignores if method patched by others, expecting that there is a postfix already applied
      var hash = AgilityPatchShared.CalculatePureSpeedMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(CalculatePureSpeedHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(AgilityPatchShared.CalculatePureSpeedMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    public override void Reset() {
    }

    private static void Postfix(
      DefaultPartySpeedCalculatingModel __instance,
      ref MobileParty mobileParty,
      ref StatExplainer explanation,
      ref int additionalTroopOnFootCount,
      ref int additionalTroopOnHorseCount,
      ref float __result) {
      // Only calculate if party leader is Khuzait (should apply to NPC lords)
      var feat = DefaultFeats.Cultural.KhuzaitCavalryAgility;
      if (mobileParty.Leader != null &&
        mobileParty.Leader.GetFeatValue(feat)) {
        // Get private methods needed for calculation

        // Recalculate Cavalry and Footmen on horses speed modifiers based on 
        var menCount = mobileParty.MemberRoster.TotalManCount + additionalTroopOnFootCount +
          additionalTroopOnHorseCount;

        var baseNumber = CalculateBaseSpeedForParty(__instance, menCount);
        var horsemenCount = mobileParty.Party.NumberOfMenWithHorse + additionalTroopOnHorseCount;
        var footmenCount = mobileParty.Party.NumberOfMenWithoutHorse + additionalTroopOnFootCount;
        var numberOfAvailableMounts =
          mobileParty.ItemRoster.NumberOfMounts; // Do this instead of calling AddCargoStats()

        if (mobileParty.AttachedParties.Count != 0)
          foreach (var attachedParty in mobileParty.AttachedParties) {
            menCount += attachedParty.MemberRoster.TotalManCount;
            horsemenCount += attachedParty.Party.NumberOfMenWithHorse;
            footmenCount += attachedParty.Party.NumberOfMenWithoutHorse;
            numberOfAvailableMounts +=
              attachedParty.ItemRoster.NumberOfMounts; // Do this instead of calling AddCargoStats()
          }

        var minFootmenCountNumberOfAvailableMounts = Math.Min(footmenCount, numberOfAvailableMounts);

        var cavalryRatioModifier = GetCavalryRatioModifier(__instance, menCount, horsemenCount);
        var mountedFootmenRatioModifier = GetMountedFootmenRatioModifier(__instance, menCount, minFootmenCountNumberOfAvailableMounts);

        // calculate Khuzait bonus and apply
        var khuzaitBonus =
          AgilityPatchShared.GetEffectBonus(feat)
          * (cavalryRatioModifier + mountedFootmenRatioModifier)
          * baseNumber;

        var explainedNumber = new ExplainedNumber(__result, explanation);

        explainedNumber.Add(khuzaitBonus, feat.Name);
        __result = explainedNumber.ResultNumber;
      }
    }

  }

}