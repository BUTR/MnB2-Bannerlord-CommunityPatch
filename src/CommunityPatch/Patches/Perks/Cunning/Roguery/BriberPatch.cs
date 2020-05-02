using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.VillageBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class BriberPatch : PerkPatchBase<BriberPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo VillagerBribeTargetMethodInfo = typeof(VillagerCampaignBehavior).GetMethod("IsBribeFeasible", NonPublic | DeclaredOnly | Instance);

    private static readonly MethodInfo VillagerSurrenderTargetMethodInfo = typeof(VillagerCampaignBehavior).GetMethod("IsSurrenderFeasible", NonPublic | DeclaredOnly | Instance);

    private static readonly MethodInfo CaravansBribeTargetMethodInfo = typeof(CaravansCampaignBehavior).GetMethod("IsBribeFeasible", NonPublic | DeclaredOnly | Instance);

    private static readonly MethodInfo CaravansSurrenderTargetMethodInfo = typeof(CaravansCampaignBehavior).GetMethod("IsSurrenderFeasible", NonPublic | DeclaredOnly | Instance);

    private static readonly MethodInfo VillagerBribePatchMethodInfo = typeof(BriberPatch).GetMethod(nameof(PrefixVillageBribe), Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo VillagerSurrenderPatchMethodInfo = typeof(BriberPatch).GetMethod(nameof(PrefixVillageSurrender), Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo CaravansBribePatchMethodInfo = typeof(BriberPatch).GetMethod(nameof(PrefixCaravansBribe), Public | NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo CaravansSurrenderPatchMethodInfo = typeof(BriberPatch).GetMethod(nameof(PrefixCaravansSurrender), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return VillagerBribeTargetMethodInfo;
      yield return VillagerSurrenderTargetMethodInfo;
      yield return CaravansBribeTargetMethodInfo;
      yield return CaravansSurrenderTargetMethodInfo;
    }

    private static readonly byte[][] VillagerBribeHashes = {
      new byte[] {
        // e1.1.0.225190
        0x34, 0xFC, 0xFA, 0xC6, 0x4B, 0x12, 0xCB, 0x84,
        0x1C, 0xA0, 0x96, 0xF4, 0xEC, 0x2B, 0xD4, 0x21,
        0x0D, 0xF3, 0x30, 0x4B, 0x96, 0x62, 0x92, 0x32,
        0xBF, 0x96, 0xDF, 0x03, 0xA1, 0xEA, 0xAE, 0x19
      }
    };

    private static readonly byte[][] VillagerSurrenderHashes = {
      new byte[] {
        // e1.1.0.225190
        0x62, 0x63, 0xC2, 0x33, 0xD9, 0xEB, 0x09, 0xBA,
        0xE5, 0xEF, 0xCF, 0x7A, 0xBC, 0xFF, 0x49, 0x45,
        0x16, 0xF4, 0x67, 0x5A, 0x01, 0x68, 0xFA, 0xDC,
        0xEC, 0xA3, 0x9B, 0xB0, 0x3B, 0x67, 0x39, 0x5A
      }
    };

    private static readonly byte[][] CaravansBribeHashes = {
      new byte[] {
        // e1.1.0.225190
        0x09, 0x11, 0xA8, 0x72, 0x94, 0xC0, 0x05, 0x88,
        0x61, 0xD6, 0x44, 0x6A, 0xEF, 0x0B, 0xC8, 0xD4,
        0x34, 0xAE, 0xA4, 0xBE, 0xAC, 0x70, 0x40, 0x00,
        0x3C, 0xB5, 0x24, 0xCB, 0x11, 0x97, 0x44, 0x54
      }
    };

    private static readonly byte[][] CaravansSurrenderHashes = {
      new byte[] {
        // e1.1.0.225190
        0x07, 0xAC, 0x8C, 0x34, 0x03, 0x89, 0x95, 0xDD,
        0x73, 0xAE, 0xC9, 0xBB, 0x9B, 0xC6, 0x5F, 0xB3,
        0xD9, 0x2C, 0x4B, 0x75, 0xDC, 0x5A, 0xFF, 0xA3,
        0x05, 0xEB, 0x09, 0x64, 0x54, 0xC5, 0x27, 0xC7
      }
    };

public BriberPatch() : base("5Trq1mQL") {}

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var villagerBribePatchInfo = Harmony.GetPatchInfo(VillagerBribeTargetMethodInfo);
      if (AlreadyPatchedByOthers(villagerBribePatchInfo)) return false;

      var villagerSurrenderPatchInfo = Harmony.GetPatchInfo(VillagerSurrenderTargetMethodInfo);
      if (AlreadyPatchedByOthers(villagerSurrenderPatchInfo)) return false;

      var caravansBribePatchInfo = Harmony.GetPatchInfo(CaravansBribeTargetMethodInfo);
      if (AlreadyPatchedByOthers(caravansBribePatchInfo)) return false;

      var caravansSurrenderPatchInfo = Harmony.GetPatchInfo(CaravansSurrenderTargetMethodInfo);
      if (AlreadyPatchedByOthers(caravansSurrenderPatchInfo)) return false;

      var villagerBribeHash = VillagerBribeTargetMethodInfo.MakeCilSignatureSha256();
      var villagerSurrenderHash = VillagerSurrenderTargetMethodInfo.MakeCilSignatureSha256();
      var caravansBribeHash = CaravansBribeTargetMethodInfo.MakeCilSignatureSha256();
      var caravansSurrenderHash = CaravansSurrenderTargetMethodInfo.MakeCilSignatureSha256();
      return villagerBribeHash.MatchesAnySha256(VillagerBribeHashes) &&
        villagerSurrenderHash.MatchesAnySha256(VillagerSurrenderHashes) &&
        caravansBribeHash.MatchesAnySha256(CaravansBribeHashes) &&
        caravansSurrenderHash.MatchesAnySha256(CaravansSurrenderHashes);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(15f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(VillagerBribeTargetMethodInfo, new HarmonyMethod(VillagerBribePatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(VillagerSurrenderTargetMethodInfo, new HarmonyMethod(VillagerSurrenderPatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(CaravansBribeTargetMethodInfo, new HarmonyMethod(CaravansBribePatchMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(CaravansSurrenderTargetMethodInfo, new HarmonyMethod(CaravansSurrenderPatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool PrefixVillageBribe(ref bool __result) {
      __result = Bribe(3, .05f, .4f);
      return false;
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool PrefixVillageSurrender(ref bool __result) {
      __result = Bribe(4, .05f, .1f);
      return false;
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool PrefixCaravansBribe(ref bool __result) {
      __result = Bribe(4, .1f, .6f);
      return false;
    }

    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool PrefixCaravansSurrender(ref bool __result) {
      __result = Bribe(7, .1f, .1f);
      return false;
    }

    private static bool Bribe(int randomIndex, float phaseOneAcceptablePowerRatio, float phaseTwoAcceptablePowerRatio) {
      var isLogicalSurrender = PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty, phaseOneAcceptablePowerRatio);
      var bribeSuccessChances = isLogicalSurrender ? 67 : 33;
      var perk = ActivePatch.Perk;

      if (MobileParty.MainParty.LeaderHero?.GetPerkValue(perk) == true)
        bribeSuccessChances += (int) perk.PrimaryBonus;

      if (MobileParty.ConversationParty.Party.Random.GetValue(randomIndex) > bribeSuccessChances) return false;

      return PartyBaseHelper.DoesSurrenderIsLogicalForParty(
        MobileParty.ConversationParty,
        MobileParty.MainParty,
        phaseTwoAcceptablePowerRatio);
    }

  }

}