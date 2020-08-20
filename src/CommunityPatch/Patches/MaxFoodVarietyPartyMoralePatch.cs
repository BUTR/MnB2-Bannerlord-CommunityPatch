// As of e1.4.1 Food variety party morale seems to work correctly 

#if !AFTER_E1_4_3

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public sealed class MaxFoodVarietyPartyMoralePatch : PatchBase<MaxFoodVarietyPartyMoralePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MaxFoodVarietyPartyMoralePatch).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x7B, 0x08, 0xE2, 0x53, 0x78, 0x94, 0x9A, 0x7A,
        0x5D, 0x36, 0x5F, 0xA4, 0x39, 0xF2, 0xF0, 0xE9,
        0xE5, 0x6E, 0x08, 0x46, 0xD0, 0x30, 0x94, 0x58,
        0x1F, 0xF6, 0x18, 0xBC, 0x19, 0xCC, 0x0C, 0xAB
      },
      new byte[] {
        // e1.3.0.227640
        0x50, 0xEF, 0xA6, 0xFB, 0x7A, 0x09, 0x7C, 0x5B,
        0x29, 0xF4, 0x5C, 0xA4, 0xB4, 0x13, 0xE6, 0x8C,
        0x34, 0x53, 0xE2, 0x77, 0x82, 0x71, 0x38, 0x24,
        0x20, 0xEA, 0x58, 0xE1, 0x2C, 0x78, 0x54, 0xDA
      },
      new byte[] {
        // e1.4.1.230527
        0xE3, 0x67, 0xDD, 0x55, 0x7E, 0x1C, 0x7C, 0x5D,
        0x51, 0x84, 0x4A, 0x20, 0xDC, 0x62, 0x87, 0x1C,
        0x2F, 0xBA, 0x8C, 0x32, 0x8B, 0x7A, 0x82, 0x43,
        0x56, 0x13, 0x39, 0xD0, 0x4A, 0xC2, 0x12, 0x32
      }
    };

    private static TextObject _foodBonusMoraleText;

    public override void Reset()
      => _foodBonusMoraleText = GameTexts.FindText("str_food_bonus_morale");

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    private static bool Prefix(MobileParty party, ref ExplainedNumber result) {
      var x = party.ItemRoster.FoodVariety;
      var y = (float)Math.Round(1.37 * x - 1.11 * Math.Sqrt(x) - 2.11, MidpointRounding.AwayFromZero);
      result.Add(y, _foodBonusMoraleText);
      return false;
    }

  }

}

#endif