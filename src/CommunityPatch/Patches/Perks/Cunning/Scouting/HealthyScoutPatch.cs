using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {

  public sealed class HealthyScoutPatch : PerkPatchBase<HealthyScoutPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultCharacterStatsModel)
      .GetMethod(nameof(DefaultCharacterStatsModel.MaxHitpoints), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(HealthyScoutPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x92, 0xC3, 0x48, 0x33, 0x3C, 0x1A, 0x39, 0x52,
        0xE5, 0x8B, 0xE4, 0x3E, 0xBD, 0x86, 0xFC, 0xA7,
        0x56, 0x27, 0xD9, 0x3A, 0xA5, 0x53, 0xC4, 0xDF,
        0xE2, 0x7B, 0x97, 0xBC, 0xB1, 0xAE, 0x36, 0x5E
      },
      new byte[] {
        // e1.4.0.228531
        0x4D, 0xB1, 0xF9, 0x12, 0x70, 0x78, 0x16, 0xBB,
        0x6C, 0xB7, 0xCE, 0x79, 0xB7, 0x23, 0xCE, 0x20,
        0x21, 0xE6, 0xC6, 0x46, 0x98, 0x30, 0x03, 0x29,
        0xD8, 0xB5, 0xC4, 0x6E, 0xAB, 0x6D, 0x8E, 0x8C
      },
      new byte[] {
        // e1.4.1.229326
        0x21, 0x79, 0x8E, 0x0E, 0xBB, 0x76, 0x56, 0x24,
        0x3C, 0x1F, 0x6E, 0x44, 0x2D, 0xA8, 0x79, 0x5F,
        0x3A, 0xA9, 0xFF, 0xA3, 0x35, 0x00, 0xDC, 0x23,
        0x02, 0x89, 0x3C, 0x04, 0x8C, 0x52, 0xFB, 0x73
      },
      new byte[] {
        // e1.4.2.231233
        0x3A, 0x9E, 0xBC, 0xBD, 0xD6, 0xA0, 0x04, 0xBD,
        0xB2, 0x6A, 0x51, 0xF7, 0xCA, 0x74, 0xDC, 0xF5,
        0xBD, 0x77, 0x0E, 0x4F, 0xD4, 0x8C, 0xF2, 0x76,
        0xB4, 0xED, 0x9E, 0x0D, 0x9A, 0x91, 0xD2, 0x90
      },
      new byte[] {
        // e1.4.3.237794
        0xD6, 0x6A, 0x16, 0x52, 0x4A, 0x02, 0xE0, 0x9F,
        0x75, 0xC1, 0x0D, 0x9A, 0xD8, 0x19, 0x6C, 0xE0,
        0x31, 0xE9, 0xF6, 0x97, 0x8F, 0x03, 0xFC, 0xDC,
        0x58, 0xCD, 0x04, 0xB8, 0x9E, 0x0D, 0x13, 0xCE
      }
    };

    public HealthyScoutPatch() : base("dDKOoD3e") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      try {
        if (!(Perk.PrimaryRole == SkillEffect.PerkRole.PartyMember
          && Perk.PrimaryBonus == 0.15f))
          return null;
      }
      catch (Exception) {
        return null;
      }

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.Modify(SkillEffect.PerkRole.Personal, 8f, SkillEffect.EffectIncrementType.Add);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(ref int __result, CharacterObject character, StatExplainer explanation) {
      var result = __result;

      var explainedNumber = new ExplainedNumber(result, explanation);

      var perk = ActivePatch.Perk;

      PerkHelper.AddPerkBonusForCharacter(perk, character, ref explainedNumber);

      __result = MBMath.Round(explainedNumber.ResultNumber);
    }

  }

}