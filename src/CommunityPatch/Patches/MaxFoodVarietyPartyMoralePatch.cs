using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public sealed class MaxFoodVarietyPartyMoralePatch : PatchBase<MaxFoodVarietyPartyMoralePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(DefaultPartyMoraleModel).GetMethod("CalculateFoodVarietyMoraleBonus", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MaxFoodVarietyPartyMoralePatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x7B, 0x08, 0xE2, 0x53, 0x78, 0x94, 0x9A, 0x7A,
        0x5D, 0x36, 0x5F, 0xA4, 0x39, 0xF2, 0xF0, 0xE9,
        0xE5, 0x6E, 0x08, 0x46, 0xD0, 0x30, 0x94, 0x58,
        0x1F, 0xF6, 0x18, 0xBC, 0x19, 0xCC, 0x0C, 0xAB
      }
    };

    public override void Reset() {
    }

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
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    
    private static void Postfix(MobileParty party, ref ExplainedNumber result) {
      if (party.ItemRoster.FoodVariety > 10)
        result.Add(party.ItemRoster.FoodVariety - 4f, GameTexts.FindText("str_food_bonus_morale"));
    }

  }

}