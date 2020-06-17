using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 4, 2)]
  public sealed class TalkToOtherLordsInAnArmyPatch : PatchBase<TalkToOtherLordsInAnArmyPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(PlayerEncounter)
        .GetMethod("DoMeetingInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(TalkToOtherLordsInAnArmyPatch)
      .GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0x18, 0xC5, 0x16, 0xD2, 0x46, 0x07, 0xBB, 0x08,
        0xAE, 0xC3, 0x6D, 0xE6, 0xFA, 0xCD, 0x2E, 0x63,
        0x0E, 0x1A, 0x73, 0xB6, 0x76, 0xF9, 0xAD, 0x0E,
        0x3F, 0xD9, 0xFA, 0x7A, 0x68, 0xFB, 0xEB, 0xE7
      },
      new byte[] {
        // e1.3.0.227640
        0x4F, 0xDD, 0xEF, 0x54, 0x13, 0xCB, 0xBB, 0x10,
        0xC1, 0x77, 0xF8, 0xF4, 0x11, 0xEF, 0x97, 0x40,
        0x26, 0x88, 0x96, 0x76, 0xA2, 0x57, 0x9C, 0x50,
        0xF5, 0xF6, 0x2C, 0x2C, 0xDA, 0x9E, 0x47, 0x21
      },
      new byte[] {
        // e1.4.1.229326
        0x84, 0xB6, 0x19, 0xB6, 0xC3, 0xB4, 0x27, 0x63,
        0x8C, 0x8A, 0x82, 0x38, 0x52, 0xB9, 0x17, 0x3E,
        0x4C, 0x7B, 0x54, 0x23, 0x56, 0x1E, 0x95, 0xF9,
        0xE1, 0x07, 0x74, 0xEF, 0x04, 0xD7, 0x92, 0xDC
      },
    };

    public override void Reset() {
    }

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        prefix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      if (EncounteredPartyField == null || MapEventStateField == null || StateHandledField == null || DefenderPartyField == null || MeetingDoneField == null) {
        CommunityPatchSubModule.Error($"{nameof(TalkToOtherLordsInAnArmyPatch)}: Could not locate all of necessary private fields for patching." + Environment.NewLine);
        return false;
      }

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    private static readonly FieldInfo EncounteredPartyField = typeof(PlayerEncounter).GetField("_encounteredParty", Instance | NonPublic);

    private static readonly FieldInfo MapEventStateField = typeof(PlayerEncounter).GetField("_mapEventState", Instance | NonPublic);

    private static readonly FieldInfo StateHandledField = typeof(PlayerEncounter).GetField("_stateHandled", Instance | NonPublic);

    private static readonly FieldInfo DefenderPartyField = typeof(PlayerEncounter).GetField("_defenderParty", Instance | NonPublic);

    private static readonly FieldInfo MeetingDoneField = typeof(PlayerEncounter).GetField("_meetingDone", Instance | NonPublic);

    private static bool Prefix(PlayerEncounter __instance) {
      if (!CommunityPatchSubModule.EnableTalkToOtherLordsInAnArmy)
        return true;

      var attacker = (PartyBase) EncounteredPartyField.GetValue(__instance);
      if (attacker.IsSettlement)
        foreach (var defender in MobileParty.MainParty.MapEvent.DefenderSide.Parties) {
          if (defender.IsSettlement)
            continue;

          attacker = defender;
          break;
        }

      if (attacker.MobileParty?.Army == null)
        return true;

      var defenderParty = (PartyBase) DefenderPartyField.GetValue(__instance);

      if (FactionManager.IsAtWarAgainstFaction(attacker.MapFaction, defenderParty.MapFaction))
        return true;

      Campaign.Current.CurrentConversationContext = ConversationContext.PartyEncounter;

      MapEventStateField.SetValue(__instance, PlayerEncounterState.Begin);
      StateHandledField.SetValue(__instance, true);

      if (!PlayerEncounter.PlayerIsAttacker)
        return false;
      var defenderMobileParty = defenderParty.IsMobile ? defenderParty.MobileParty : null;
      var defenderArmy = defenderMobileParty?.Army;
      if (defenderArmy != null && defenderArmy.LeaderParty == defenderMobileParty
        && !defenderArmy.LeaderParty.AttachedParties.Contains(MobileParty.MainParty))
        GameMenu.SwitchToMenu("army_encounter");
      else {
        MeetingDoneField.SetValue(__instance, true);
        CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty),
          new ConversationCharacterData(attacker.Leader, attacker));
      }

      return false;
    }

  }

}