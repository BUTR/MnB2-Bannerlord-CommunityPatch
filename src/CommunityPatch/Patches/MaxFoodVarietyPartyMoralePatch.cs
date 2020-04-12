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
        // e1.1.0.224785
        0xDF, 0x45, 0x2D, 0x05, 0xD9, 0xBB, 0xCF, 0x84,
        0x03, 0xE1, 0xA6, 0x0B, 0x5D, 0x49, 0xC8, 0xC1,
        0x69, 0xC5, 0x31, 0x0F, 0x4E, 0x8D, 0xFC, 0x83,
        0x54, 0xA6, 0x43, 0xDD, 0xEC, 0xDE, 0xB2, 0x17
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