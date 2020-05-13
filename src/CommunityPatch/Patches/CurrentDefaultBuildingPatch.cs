using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityPatch;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;
using static System.Reflection.BindingFlags;

namespace Patches {

  public sealed class CurrentDefaultBuildingPatch : PatchBase<CurrentDefaultBuildingPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Town).GetMethod("get_CurrentDefaultBuilding", Public | NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(CurrentDefaultBuildingPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override void Reset() {
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.4.0.228869
        0xFD, 0x41, 0x75, 0x89, 0x94, 0x91, 0xB3, 0xD1,
        0x85, 0x23, 0xC9, 0x32, 0x66, 0x7F, 0xD8, 0xB5,
        0x75, 0xA0, 0x24, 0xD0, 0x6E, 0x10, 0x44, 0x9C,
        0xBD, 0x64, 0x2B, 0x75, 0x9F, 0x38, 0x33, 0x69
      }
    };

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    private static void Postfix(Town __instance, ref Building __result) {
      if (__result != null)
        return;

      __result =
        __instance.Buildings
          .Where(b => b.BuildingType.IsDefaultProject)
          .ToList()
          .GetRandomElement();

      if (__result == null)
        // TODO: find some default building and add it to the list of buildings and then pick it
        throw new NotImplementedException("No default buildings to pick from!");
    }

  }

}