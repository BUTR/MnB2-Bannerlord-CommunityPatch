using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public class HorseGroomingPatch : PatchBase<HorseGroomingPatch> {

     public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultVillageProductionCalculatorModel)
      .GetMethod(nameof(DefaultVillageProductionCalculatorModel.CalculateDailyProductionAmount), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(HorseGroomingPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.2.1.227410
        0xD6, 0xD4, 0xA4, 0xA8, 0x19, 0xEF, 0xE1, 0x6F,
        0x4F, 0x10, 0x2D, 0xDB, 0x40, 0x3A, 0x0B, 0x94,
        0x39, 0x17, 0x34, 0x5A, 0x32, 0x73, 0x9B, 0x1D,
        0x8C, 0xED, 0xE0, 0xAF, 0xBF, 0xD4, 0x0C, 0xE2
      }
    };

    public override void Reset() {}

    public override bool? IsApplicable(Game game)
      => TargetMethodInfo != null
        && !AlreadyPatchedByOthers(Harmony.GetPatchInfo(TargetMethodInfo))
        && TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes[0]);

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(Village village, ItemObject item, ref float __result) {
      if (village.VillageState != Village.VillageStates.Normal || !item.IsMountable) {
        return;
      }

      if (village.Bound.Town.Owner?.Settlement.OwnerClan?.Leader.GetPerkValue(DefaultPerks.Riding.HorseGrooming) ?? false) {
        __result = village.VillageType.Productions
          .Where(productionItemTuple => productionItemTuple.Item1 == item)
          .Select(productionItemTuple => productionItemTuple.Item2)
          .Aggregate(0, (float acc, float currentProductionValue)
            => acc + currentProductionValue * (1 + DefaultPerks.Riding.HorseGrooming.PrimaryBonus));
      }
    }
  }
}