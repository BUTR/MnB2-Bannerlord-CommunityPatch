using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class ScavengerPatch : PerkPatchBase<ScavengerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = LootCollectorHelper.GiveShareOfLootToPartyMethod;

    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(ScavengerPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x6E, 0x32, 0xE7, 0x07, 0x5A, 0xEC, 0x54, 0x10,
        0x9D, 0x9E, 0x0E, 0x6F, 0xF2, 0x82, 0x53, 0xB1,
        0xE0, 0x6D, 0x90, 0x47, 0x0A, 0x88, 0x59, 0x8F,
        0x28, 0x63, 0x8F, 0x51, 0x94, 0x2A, 0x8C, 0x5F
      },
      new byte[] {
        // e1.3.0.227640
        0xFA, 0x09, 0x52, 0x78, 0x52, 0xBF, 0x11, 0x77,
        0x03, 0x29, 0xC5, 0x18, 0x80, 0xB9, 0x5A, 0x2C,
        0xC3, 0xF4, 0x96, 0xB5, 0x5E, 0xEA, 0x5B, 0x7F,
        0x9B, 0x49, 0x01, 0x9C, 0x73, 0x5C, 0xC4, 0x69
      },
      new byte[] {
        // e1.4.0.228531
        0xD1, 0x67, 0xC7, 0x37, 0x43, 0xF3, 0x61, 0xFE,
        0x00, 0xFB, 0xE6, 0x1B, 0xC8, 0x00, 0x13, 0x53,
        0x1C, 0xB3, 0x32, 0xAB, 0x04, 0x47, 0x1E, 0xC7,
        0x78, 0xE9, 0x16, 0x0D, 0xCF, 0x1E, 0xA6, 0x05
      },
      new byte[] {
        // e1.4.1.229326
        0xAA, 0xBF, 0x2B, 0xD6, 0x9D, 0xDC, 0x96, 0x14,
        0x19, 0x55, 0x8D, 0x37, 0x75, 0x98, 0x0D, 0x8F,
        0x7E, 0xFC, 0x26, 0xEB, 0x73, 0x7F, 0xA1, 0xEB,
        0x3A, 0xF0, 0x44, 0x52, 0xAD, 0xC6, 0xE9, 0xBA
      },
      new byte[] {
        // e1.4.2.231233
        0xF7, 0xA4, 0xBF, 0x82, 0xD8, 0x54, 0x93, 0xC3,
        0x02, 0x1D, 0x08, 0x4B, 0x8F, 0x11, 0xEE, 0x82,
        0x20, 0xB0, 0xB5, 0xCC, 0x57, 0x7D, 0x7B, 0x80,
        0x45, 0x31, 0x66, 0x76, 0xDF, 0xA6, 0x44, 0x91
      },
      new byte[] {
        // e1.4.3.237794
        0x1A, 0x10, 0xC0, 0x73, 0x49, 0x6C, 0xB9, 0x3D,
        0xE8, 0x67, 0xED, 0xAA, 0x58, 0xC4, 0xB4, 0x04,
        0x9F, 0xF2, 0xA8, 0x4D, 0x53, 0x0F, 0x9A, 0x83,
        0xD8, 0xEA, 0x66, 0x5F, 0x89, 0xFD, 0xA1, 0x32
      },
      new byte[] {
        // e1.5.1.241359
        0x41, 0xDB, 0x91, 0xC3, 0x45, 0x09, 0x29, 0x47,
        0xB5, 0xB0, 0x9F, 0x52, 0x9E, 0x86, 0x7E, 0x6F,
        0x7B, 0x45, 0xA8, 0x61, 0x06, 0x3C, 0x99, 0x7D,
        0x36, 0x9D, 0x05, 0xFF, 0x4A, 0x3F, 0x63, 0x2A
      }
    };

    public ScavengerPatch() : base("cYjeJTb8") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(25f);
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfoPrefix));
      Applied = true;
    }

    public static void Prefix(ref object __instance, PartyBase partyToReceiveLoot, PartyBase winnerParty, float lootAmount) {
      var lootFactor = CalculateLootFactor(winnerParty);
      if (lootFactor.IsZero()) return;

      var casualtiesInBattle = LootCollectorHelper.GetCasualtiesInBattle(__instance);
      var casualtiesShare = GetCasualtiesShare(lootAmount, casualtiesInBattle);
      DistributeCasualtiesLoot(__instance, partyToReceiveLoot, winnerParty, casualtiesShare, lootFactor);
    }

    private static float CalculateLootFactor(PartyBase winnerParty) {
      var explainedNumber = new ExplainedNumber(1f);

      if (winnerParty.MobileParty != null)
#if AFTER_E1_5_1
        PerkHelper.AddPerkBonusForParty(ActivePatch.Perk, winnerParty.MobileParty, true, ref explainedNumber);
#else
        PerkHelper.AddPerkBonusForParty(ActivePatch.Perk, winnerParty.MobileParty, ref explainedNumber);
#endif

      return explainedNumber.ResultNumber - explainedNumber.BaseNumber;
    }

    private static List<TroopRosterElement> GetCasualtiesShare(float lootAmount, TroopRoster casualtiesInBattle) {
      var casualtiesShare = new List<TroopRosterElement>();

      for (var i = casualtiesInBattle.Count - 1; i >= 0; i--) {
        var troopSize = casualtiesInBattle.GetElementNumber(i);
        var troop = casualtiesInBattle.GetCharacterAtIndex(i);
        var emptyTroop = new TroopRosterElement(troop);

        for (var trooper = 0; trooper < troopSize; trooper++) {
          if (!(MBRandom.RandomFloat < lootAmount)) continue;

          casualtiesShare.Add(emptyTroop);
        }
      }

      return casualtiesShare;
    }

    private static void DistributeCasualtiesLoot(object __instance, PartyBase partyToReceiveLoot, PartyBase winnerParty, List<TroopRosterElement> casualties, float lootFactor) {
      if (winnerParty == PartyBase.MainParty) {
        LootCasualtiesForPlayer(__instance, partyToReceiveLoot, casualties, lootFactor);
        return;
      }

      LootCasualtiesForAi(__instance, partyToReceiveLoot, casualties, lootFactor);
    }

    private static void LootCasualtiesForPlayer(object __instance, PartyBase partyToReceiveLoot, List<TroopRosterElement> casualties, float lootFactor) {
      var playerLoot = LootCollectorHelper.LootCasualties(__instance, casualties, lootFactor);
      partyToReceiveLoot.ItemRoster.Add(playerLoot);
    }

    private static void LootCasualtiesForAi(object __instance, PartyBase partyToReceiveLoot, List<TroopRosterElement> casualties, float lootFactor) {
      if (partyToReceiveLoot.LeaderHero == null) return;

      var loot = LootCollectorHelper.LootCasualties(__instance, casualties, 0.5f);
      var gold = LootCollectorHelper.ConvertLootToGold(loot);
      gold = MBMath.Round(gold * 0.5f * lootFactor);
      GiveGoldAction.ApplyBetweenCharacters(null, partyToReceiveLoot.LeaderHero, gold);
    }

  }

}