#if !AFTER_E1_4_3

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public class HorseGroomingPatch : PerkPatchBase<HorseGroomingPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultVillageProductionCalculatorModel)
      .GetMethod(nameof(DefaultVillageProductionCalculatorModel.CalculateDailyProductionAmount), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(HorseGroomingPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.2.1.227410
        0xD6, 0xD4, 0xA4, 0xA8, 0x19, 0xEF, 0xE1, 0x6F,
        0x4F, 0x10, 0x2D, 0xDB, 0x40, 0x3A, 0x0B, 0x94,
        0x39, 0x17, 0x34, 0x5A, 0x32, 0x73, 0x9B, 0x1D,
        0x8C, 0xED, 0xE0, 0xAF, 0xBF, 0xD4, 0x0C, 0xE2
      },
      new byte[] {
        // e1.4.0.228531
        0x64, 0xB3, 0x05, 0xE4, 0x95, 0x58, 0xB4, 0xAB,
        0x0E, 0xC4, 0x73, 0xED, 0x34, 0xD7, 0xFE, 0xFF,
        0xE1, 0xFF, 0xCA, 0x39, 0xAD, 0x54, 0x43, 0xA7,
        0x52, 0xF4, 0x2B, 0x09, 0x90, 0xDD, 0xAD, 0xBB
      }
    };

    public HorseGroomingPatch() : base("wtyLhmz5") {
    }

    public override bool? IsApplicable(Game game) {
      if (TargetMethodInfo == null
        || AlreadyPatchedByOthers(Harmony.GetPatchInfo(TargetMethodInfo))
        || !TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    private static void Postfix(Village village, ItemObject item, ref float __result) {
      if (village?.VillageState != Village.VillageStates.Normal || !(item?.IsMountable ?? false))
        return;

      if (!(village.Bound?.Town?.Owner?.Settlement?.OwnerClan?.Leader?.GetPerkValue(ActivePatch.Perk) ?? false))
        return;

      var production = village.VillageType?.Productions
        ?.Where(productionItemTuple => productionItemTuple.Item1 == item)
        .Select(productionItemTuple => productionItemTuple.Item2)
        .Aggregate(0, (float acc, float currentProductionValue)
          => acc + currentProductionValue * (1 + ActivePatch.Perk.PrimaryBonus));

      __result = production ?? __result;
    }

  }

}

#endif