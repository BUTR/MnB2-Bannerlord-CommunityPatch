using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.PatchApplicabilityHelper;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class ElusivePatch : PatchBase<ElusivePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo SacrificeTargetMethodInfo = typeof(DefaultTroopSacrificeModel).GetMethod(nameof(DefaultTroopSacrificeModel.GetNumberOfTroopsSacrificedForTryingToGetAway), Public | Instance | DeclaredOnly);
    private static readonly MethodInfo BreakOutTargetMethodInfo = typeof(DefaultTroopSacrificeModel).GetMethod(nameof(DefaultTroopSacrificeModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement), Public | Instance | DeclaredOnly);
    private static readonly MethodInfo BreakInTargetMethodInfo = typeof(DefaultTroopSacrificeModel).GetMethod(nameof(DefaultTroopSacrificeModel.GetLostTroopCountForBreakingInBesiegedSettlement), Public | Instance | DeclaredOnly);
    private static readonly MethodInfo SiegePatchMethodInfo = typeof(ElusivePatch).GetMethod(nameof(SiegePostfix), Public | NonPublic | Static | DeclaredOnly);
    private static readonly MethodInfo SacrificePatchMethodInfo = typeof(ElusivePatch).GetMethod(nameof(SacrificePostfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return SacrificeTargetMethodInfo;
      yield return BreakOutTargetMethodInfo;
      yield return BreakInTargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] SacrificeHashes = {
      new byte[] {
        // e1.2.1.226961
        0x36, 0x69, 0x6E, 0xC5, 0x43, 0xA7, 0xF4, 0x0F,
        0x0A, 0x98, 0xB6, 0xC2, 0x96, 0x63, 0x43, 0x24,
        0x5A, 0xC4, 0x4A, 0x65, 0x28, 0x72, 0xB0, 0x59,
        0xEF, 0x1B, 0x96, 0x03, 0xE2, 0xBE, 0x32, 0x21
      }
    };
    
    private static readonly byte[][] BreakOutHashes = {
      new byte[] {
        // e1.2.1.226961
        0x0A, 0x4A, 0x41, 0xC1, 0xA6, 0x01, 0xC1, 0x8B,
        0x9A, 0xD8, 0x0D, 0xFD, 0x6B, 0x1E, 0x6A, 0x6C,
        0x9A, 0x1B, 0x15, 0x14, 0xCF, 0x55, 0x28, 0xF0,
        0x44, 0x2B, 0x75, 0xE3, 0x7B, 0xD7, 0xA6, 0xEA
      }
    };
    
    private static readonly byte[][] BreakInHashes = {
      new byte[] {
        // e1.2.1.226961
        0x16, 0x46, 0x40, 0x13, 0x98, 0x24, 0x44, 0xB0,
        0xDD, 0x70, 0xD0, 0x88, 0xF8, 0x2B, 0x22, 0x23,
        0x49, 0x2F, 0x2D, 0x41, 0x0D, 0xA6, 0xF1, 0x94,
        0xDA, 0x0B, 0xA9, 0xFD, 0xDB, 0xC2, 0x3F, 0xC4
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "iZ5hXYTl");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;

      return IsTargetPatchable(SacrificeTargetMethodInfo, SacrificeHashes) &&
        IsTargetPatchable(BreakOutTargetMethodInfo, BreakOutHashes) && 
        IsTargetPatchable(BreakInTargetMethodInfo, BreakInHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(SacrificeTargetMethodInfo, postfix: new HarmonyMethod(SacrificePatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(BreakOutTargetMethodInfo, postfix: new HarmonyMethod(SiegePatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(BreakInTargetMethodInfo, postfix: new HarmonyMethod(SiegePatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SiegePostfix(ref int __result, MobileParty party) {
      ref var troopSacrificeSize = ref __result;
      if (troopSacrificeSize < 0) return;

      var perk = ActivePatch._perk;
      if (party.LeaderHero?.GetPerkValue(perk) != true) return;

      troopSacrificeSize -= (int) (troopSacrificeSize * perk.PrimaryBonus);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SacrificePostfix(ref int __result, BattleSideEnum battleSide, MapEvent mapEvent) {
      ref var troopSacrificeSize = ref __result;
      if (troopSacrificeSize < 0) return;

      var side = battleSide == BattleSideEnum.Attacker ? mapEvent.AttackerSide : mapEvent.DefenderSide;
      var perk = ActivePatch._perk;
      var sideTotalMen = 0;
      var menInElusiveParties = 0;
      foreach (var sideParty in side.Parties) {
        sideTotalMen += sideParty.NumberOfAllMembers;
        if (sideParty.MobileParty?.LeaderHero?.GetPerkValue(perk) == true)
          menInElusiveParties += sideParty.NumberOfAllMembers;
      }
      var ratioMenInElusiveParties = menInElusiveParties / (float) sideTotalMen;
      troopSacrificeSize -= (int) (troopSacrificeSize * ratioMenInElusiveParties * perk.PrimaryBonus);
    }

  }

}