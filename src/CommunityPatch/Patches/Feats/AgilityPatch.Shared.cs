using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CommunityPatch.Patches.Feats {

  internal sealed class AgilityPatchShared {

    public static readonly MethodInfo CalculatePureSpeedMethodInfo =
      AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculatePureSpeed");

    public static readonly MethodInfo CalculateFinalSpeedMethodInfo =
      AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed");

    public static readonly byte[][] CalculatePureSpeedHashes = {
      new byte[] {
        // e1.2.0.226271 and presumably previous versions
        0x61, 0xD7, 0x4D, 0xF5, 0xB0, 0x0E, 0x84, 0x52,
        0xDC, 0xCB, 0x2F, 0xE7, 0xE2, 0x20, 0x38, 0x10,
        0x87, 0x01, 0xE3, 0x61, 0xF1, 0xAB, 0x89, 0x7D,
        0x9C, 0xDC, 0x50, 0x6E, 0xA6, 0x7E, 0xEB, 0xEF
      }
    };

    public static readonly byte[][] CalculateFinalSpeedHashes = {
      new byte[] {
        // e1.1.0.224785
        0x58, 0xF5, 0x64, 0xA2, 0xE1, 0x17, 0x5C, 0x0C,
        0x13, 0xEE, 0xED, 0xA8, 0x5E, 0xFC, 0xDF, 0x38,
        0x7E, 0xCE, 0x1E, 0xF9, 0xF8, 0x67, 0x78, 0x3F,
        0xB9, 0x71, 0xE0, 0x02, 0x8D, 0x58, 0x40, 0x99
      }
    };

    public static float GetEffectBonus(FeatObject feat) {
      var result = 0f;

      if (feat == null)
        CommunityPatchSubModule.Error("AgilityPatchShared.GetEffectBonus():  given feat is null");
      else // As of e1.3.0.226834, feat effect bonuses are stored in % instead of decimal
        // (i.e. 30f = 30%, rather than 0.30f = 30%)
        result = feat.EffectBonus < 1.0f ? feat.EffectBonus : feat.EffectBonus / 100.00f;

      return result;
    }

  }

}