using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

    public bool IsApplicable(Game game) {
      if (Applied) return false;
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

     var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x4C, 0x29, 0xDC, 0x2D, 0x78, 0x89, 0xA7, 0xA8,
        0xC6, 0xDA, 0x84, 0xDB, 0x07, 0x2E, 0x7D, 0xB4,
        0x99, 0xED, 0xB2, 0xB9, 0xC4, 0xBB, 0xAD, 0xE4,
        0xC9, 0xD1, 0xC8, 0x0F, 0xD7, 0x8C, 0x25, 0x15
      });
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
    
    public void Reset() {}

  }

}