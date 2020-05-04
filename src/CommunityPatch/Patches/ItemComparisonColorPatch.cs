using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;
using Harmony = HarmonyLib.Harmony;

namespace CommunityPatch.Patches {

  public sealed class ItemComparisonColorPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(ItemMenuVM).GetMethod("GetColorFromComparison", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ItemComparisonColorPatch).GetMethod(nameof(GetColorFromComparisonPatched), NonPublic | Static | DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x37, 0x16, 0xDE, 0xA4, 0x54, 0x11, 0x5A, 0xB1,
        0xDE, 0xD1, 0x49, 0xD8, 0x2A, 0x34, 0x62, 0x9F,
        0x57, 0x48, 0x0C, 0x19, 0xBD, 0x33, 0xF9, 0xA1,
        0xD5, 0x4C, 0xA4, 0xA6, 0xF8, 0x4B, 0x4F, 0x9D
      },
      new byte[] {
        // e1.0.5
        0xA6, 0x6C, 0x13, 0x03, 0x36, 0x3C, 0x4D, 0x65,
        0x58, 0x5A, 0x7F, 0x29, 0x2A, 0x0F, 0x9E, 0xE7,
        0xF7, 0x19, 0xDF, 0xB7, 0xDF, 0x41, 0x65, 0xEF,
        0x4D, 0xB0, 0xD8, 0x6C, 0x5E, 0xDE, 0x23, 0x96
      }
    };

    public bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    
    private static bool GetColorFromComparisonPatched(int result, bool isCompared, out Color __result) {
      if (MobileParty.MainParty == null) {
        __result = Colors.Black;
        return false;
      }

      if (result != -1) {
        if (result != 1) {
          __result = Colors.Black;
          return false;
        }

        if (!isCompared) {
          __result = UIColors.PositiveIndicator;
          return false;
        }

        __result = UIColors.NegativeIndicator;
        return false;
      }

      if (!isCompared) {
        __result = UIColors.NegativeIndicator;
        return false;
      }

      __result = UIColors.PositiveIndicator;
      return false;
    }

    public void Reset() {
    }

  }

}