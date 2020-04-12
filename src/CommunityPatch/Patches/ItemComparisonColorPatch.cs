using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static CommunityPatch.HarmonyHelpers;
using Harmony = HarmonyLib.Harmony;

namespace CommunityPatch.Patches {

  public class ItemComparisonColorPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(ItemMenuVM).GetMethod("GetColorFromComparison", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ItemComparisonColorPatch).GetMethod(nameof(GetColorFromComparisonPatched), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0xA4, 0x23, 0xB9, 0x31, 0x31, 0x29, 0x7D, 0x18,
        0x98, 0x7D, 0x65, 0xDD, 0x41, 0xA2, 0xF6, 0x42,
        0x09, 0xCE, 0xDD, 0xC2, 0xD7, 0x69, 0x23, 0x21,
        0xFE, 0x74, 0xEC, 0x85, 0xF2, 0x48, 0xD6, 0x2F
      }
    };

    public bool IsApplicable(Game game) {
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
    [MethodImpl(MethodImplOptions.NoInlining)]
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