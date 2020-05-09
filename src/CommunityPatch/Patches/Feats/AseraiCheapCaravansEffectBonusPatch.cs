using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch.Patches.Feats {

  public sealed class AseraiCheapCaravansEffectBonusPatch : PatchBase<AseraiCheapCaravansEffectBonusPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = AccessTools.Method(typeof(DefaultFeats), "InitializeAll");

    private static readonly float PatchedEffectBonusValue = 0.30f; // 30% per character creation screen

    public static readonly byte[][] TargetHashes = {
      new byte[] {
        // e1.2.0.226271
        0xA4, 0x93, 0xCF, 0x2E, 0x6F, 0x90, 0x38, 0x41,
        0x5C, 0x15, 0x2E, 0x81, 0x1A, 0xCA, 0x13, 0x87,
        0x37, 0xD2, 0x63, 0x5A, 0x06, 0xED, 0x31, 0x7F,
        0x7A, 0x99, 0x82, 0xBB, 0x1F, 0x07, 0xB3, 0xE1
      },
      new byte[] {
        // e1.3.0.227640
        0x5B, 0x5E, 0x98, 0x8A, 0xBA, 0xB1, 0x46, 0x9F,
        0xA3, 0xC5, 0x86, 0x6E, 0xE1, 0x85, 0x21, 0xF1,
        0x7A, 0x6A, 0xB5, 0xDD, 0x25, 0x8F, 0x2E, 0xF4,
        0xF9, 0x33, 0x56, 0x6D, 0x9F, 0x22, 0x45, 0x3F
      }
    };

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override bool? IsApplicable(Game game) {
      try {
        if (TargetMethodInfo == null
          || !DefaultFeats.Cultural.AseraiCheapCaravans.EffectBonus.IsDifferentFrom(PatchedEffectBonusValue)) {
          return false;
        }
      }
      catch {
        return null;
      }

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(TargetHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      var featObject =
        (FeatObject) AccessTools.Field(
          typeof(DefaultFeats), "_cultureAseraiCheapCaravans")?.GetValue(Campaign.Current.DefaultFeats);

      if (featObject == null) {
        Applied = false;
      }
      else {
        featObject.Initialize(
          featObject.Name.ToString(),
          featObject.Description.ToString(),
          PatchedEffectBonusValue,
          featObject.IncrementType);

        Applied = !featObject.EffectBonus.IsDifferentFrom(PatchedEffectBonusValue);

        if (!Applied) {
          CommunityPatchSubModule.Error("AseraiCheapCaravansEffectBonusPatch:  Feat not initialized with new EffectBonus value");
        }
      }
    }

    public override void Reset()
      => Applied = false;

  }

}