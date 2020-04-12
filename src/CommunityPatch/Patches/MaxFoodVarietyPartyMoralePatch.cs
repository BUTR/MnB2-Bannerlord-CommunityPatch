using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class MaxFoodVarietyPartyMoralePatch : PatchBase<MaxFoodVarietyPartyMoralePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MaxFoodVarietyPartyMoralePatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }
    
    private static readonly byte[][] Hashes = {
      new byte[] {
        0xd6, 0xe5, 0xcd, 0xda, 0xc0, 0xa1, 0x38, 0x58,
        0xfa, 0xb7, 0x0c, 0x92, 0xc4, 0x51, 0x1e, 0xee,
        0x5e, 0x05, 0x66, 0xe4, 0x83, 0xfc, 0xc6, 0x97,
        0x22, 0x7b, 0x0a, 0xde, 0xd5, 0x38, 0xd6, 0xc8
      },
      new byte[] {
        // e1.1.0
        0x4D, 0xA2, 0x32, 0x75, 0xFC, 0x85, 0x32, 0x39,
        0xD0, 0xBB, 0x26, 0xBE, 0xAA, 0x9A, 0x22, 0x54,
        0x03, 0x26, 0xAA, 0xEB, 0x4A, 0x21, 0xC6, 0x58,
        0x6E, 0x9E, 0xE6, 0xD9, 0xC1, 0x51, 0x1C, 0x60
      }
    };

    public override void Reset() {
    }

    public override bool IsApplicable(Game game)
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
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(MobileParty party, ref ExplainedNumber result) {
      if (party.ItemRoster.FoodVariety > 10) {
        result.Add(party.ItemRoster.FoodVariety - 4f, GameTexts.FindText("str_food_bonus_morale", (string) null));
      }
    }

  }

}