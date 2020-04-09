using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Library;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class MaxFoodVarietyPartyMoralePatch : PatchBase<MaxFoodVarietyPartyMoralePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MaxFoodVarietyPartyMoralePatch).GetMethod("Postfix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override void Reset() {
      
    }

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xd6,0xe5,0xcd,0xda,0xc0,0xa1,0x38,0x58,
        0xfa,0xb7,0x0c,0x92,0xc4,0x51,0x1e,0xee,
        0x5e,0x05,0x66,0xe4,0x83,0xfc,0xc6,0x97,
        0x22,0x7b,0x0a,0xde,0xd5,0x38,0xd6,0xc8
      });
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    public static void Postfix(ref float __result, MobileParty party, ExplainedNumber result) {
      if (party.ItemRoster.FoodVariety > 10) {
        __result = 8f;
      }
    }

  }

}
