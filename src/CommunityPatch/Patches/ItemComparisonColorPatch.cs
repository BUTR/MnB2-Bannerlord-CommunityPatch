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

namespace CommunityPatch.Patches {

  public class ItemComparisonColorPatch : IPatch {

    private static readonly MethodInfo TargetMethodInfo = typeof(ItemMenuVM).GetMethod("GetColorFromComparison", BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ItemComparisonColorPatch).GetMethod(nameof(GetColorFromComparisonPatched), BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly);

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (patchInfo.Owners.Any())
        return false;

      var bytes = TargetMethodInfo.GetMethodBody()?.GetILAsByteArray();
      if (bytes == null) return false;

      using var hasher = SHA256.Create();
      var hash = hasher.ComputeHash(bytes);
      return hash.SequenceEqual(new byte[] { });
    }

    public void Apply(Game game)
      => CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo));

    private static bool GetColorFromComparisonPatched(int result, bool isCompared, ref Color __result) {
      if (MobileParty.MainParty != null && !(MobileParty.MainParty.HasPerk(DefaultPerks.Trade.WholeSeller) || MobileParty.MainParty.HasPerk(DefaultPerks.Trade.Appraiser))) {
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

  }

}