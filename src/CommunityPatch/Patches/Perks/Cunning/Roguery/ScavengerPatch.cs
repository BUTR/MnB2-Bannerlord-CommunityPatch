using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {
  public class ScavengerPatch : PatchBase<ScavengerPatch> {
    public override bool Applied { get; protected set; }
    
    private static readonly MethodInfo TargetMethodInfo = LootCollectorHelper.GiveShareOfLootToPartyMethod;
    private static readonly MethodInfo PatchMethodInfoPrefix = typeof(ScavengerPatch).GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x6E, 0x32, 0xE7, 0x07, 0x5A, 0xEC, 0x54, 0x10,
        0x9D, 0x9E, 0x0E, 0x6F, 0xF2, 0x82, 0x53, 0xB1,
        0xE0, 0x6D, 0x90, 0x47, 0x0A, 0x88, 0x59, 0x8F,
        0x28, 0x63, 0x8F, 0x51, 0x94, 0x2A, 0x8C, 0x5F
      }
    };
    
    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "cYjeJTb8");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );
      // most of the properties of skills have private setters, yet Initialize is public
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 25f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfoPrefix));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
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
        PerkHelper.AddPerkBonusForParty(ActivePatch._perk, winnerParty.MobileParty, ref explainedNumber);

      return explainedNumber.ResultNumber - explainedNumber.BaseNumber;
    }
    
    private static List<TroopRosterElement> GetCasualtiesShare(float lootAmount, TroopRoster casualtiesInBattle) {
      var casualtiesShare = new List<TroopRosterElement>();
      
      for (var i = casualtiesInBattle.Count - 1; i >= 0; i--) {
        var troopSize = casualtiesInBattle.GetElementNumber(i);
        var troop = casualtiesInBattle.GetCharacterAtIndex(i);
        var emptyTroop = new TroopRosterElement(troop);
        
        for (var num4 = 0; num4 < troopSize; num4++) {
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