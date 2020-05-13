using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class BaitPatch : PerkPatchBase<BaitPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = PlayerEncounterHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(BaitPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static byte[][] Hashes => PlayerEncounterHelper.TargetHashes;

    public BaitPatch() : base("6MBoNlxj") {
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
      Perk.Modify(30f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref PlayerEncounter __instance, ref List<MobileParty> partiesToJoinOurSide, ref List<MobileParty> partiesToJoinEnemySide) {
      var ourParty = PartyBase.MainParty;
      var encounterParty = PlayerEncounterHelper.GetEncounteredParty(__instance);

      AddNearbyAlliesToParty(ourParty, encounterParty, partiesToJoinOurSide);
      AddNearbyAlliesToParty(encounterParty, ourParty, partiesToJoinEnemySide, true);

      partiesToJoinEnemySide = partiesToJoinEnemySide.Distinct().ToList();
      partiesToJoinOurSide = partiesToJoinOurSide.Distinct().ToList();
    }

    private static void AddNearbyAlliesToParty(PartyBase party, PartyBase enemyParty, List<MobileParty> alliesFound, bool allyMustBeAbleToAttack = false) {
      var isSiege = IsSiegeEncounter();
      var position2D = GetEncounterPosition(isSiege);
      var radius = CalculateCallToArmsRadius(party, isSiege);

      var possibleAllies = PlayerEncounterHelper.FindPartiesAroundPosition(position2D, radius);

      foreach (var possibleAlly in possibleAllies) {
        if (!IsAlly(party, enemyParty, possibleAlly, allyMustBeAbleToAttack))
          continue;

        alliesFound.Add(possibleAlly);

        if (possibleAlly.Army != null && possibleAlly.Army.LeaderParty == possibleAlly)
          alliesFound.AddRange(possibleAlly.Army.LeaderParty.AttachedParties);
      }
    }

    private static bool IsSiegeEncounter()
      => PlayerEncounter.EncounteredParty != null &&
        PlayerEncounter.EncounteredParty.IsMobile &&
        PlayerEncounter.EncounteredParty.MobileParty.BesiegedSettlement != null ||
        MobileParty.MainParty.BesiegedSettlement != null;

    private static Vec2 GetEncounterPosition(bool isSiegeEvent) {
      if (!isSiegeEvent) return MobileParty.MainParty.Position2D;

      var encountered = PlayerEncounter.EncounteredParty;

      if (encountered?.IsMobile == true && encountered?.MobileParty?.BesiegedSettlement != null) {
        return encountered.SiegeEvent?.BesiegerCamp?.BesiegerParty?.Position2D
          ?? MobileParty.MainParty.Position2D;
      }

      return MobileParty.MainParty.BesiegerCamp?.BesiegerParty?.Position2D
        ?? MobileParty.MainParty.Position2D;
    }

    private static float CalculateCallToArmsRadius(PartyBase party, bool isSiege) {
      var baseRadius = 3f * (isSiege ? 1.5f : 1f);
      var finalRadius = new ExplainedNumber(baseRadius);

      if (party.MobileParty != null)
        PerkHelper.AddPerkBonusForParty(ActivePatch.Perk, party.MobileParty, ref finalRadius);

      return finalRadius.ResultNumber;
    }

    private static bool IsAlly(PartyBase party, PartyBase enemyParty, MobileParty possibleAlly, bool allyMustBeAbleToAttack) {
      if (!IsAValidPossibleAlly(party, possibleAlly))
        return false;

      if (!MapEventHelper.PartyCanJoinSideOf(possibleAlly, party, enemyParty))
        return false;

      return !allyMustBeAbleToAttack || PlayerEncounterHelper.CanPartyAttack(possibleAlly, enemyParty.MobileParty);
    }

    private static bool IsAValidPossibleAlly(PartyBase party, MobileParty possibleAlly) {
      var mainParty = MobileParty.MainParty;
      if (possibleAlly == mainParty
        || possibleAlly == PlayerEncounter.EncounteredParty.MobileParty
        || possibleAlly.MapEvent != null
        || !possibleAlly.IsActive)
        return false;

      if (possibleAlly.BesiegedSettlement != null) {
        var mapEvent = mainParty.MapEvent;

        var allyInArmy = mainParty.Army?.LeaderParty.AttachedParties.Contains(possibleAlly) ?? false;

        if (mapEvent != null) {
          var mapEventType = mapEvent.EventType;
          var isSiege = mapEventType != MapEvent.BattleTypes.Siege
            && mapEventType != MapEvent.BattleTypes.SiegeOutside;

          if (isSiege
            || mapEvent.MapEventSettlement != possibleAlly.BesiegedSettlement
            && allyInArmy)
            return false;
        }
        else {
          if (allyInArmy)
            return false;
        }
      }

      if (possibleAlly.Army?.LeaderParty.AttachedParties.Contains(possibleAlly) ?? false)
        return false;

      var allySettlement = possibleAlly.CurrentSettlement;

      return allySettlement == null
        || mainParty.BesiegedSettlement == allySettlement
        || party.MobileParty != null
        && allySettlement == party.MobileParty.CurrentSettlement;
    }

  }

}