using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  public sealed class MenuWhenEncounteringAnArmyPatch : PatchBase<MenuWhenEncounteringAnArmyPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(PlayerEncounter)
        .GetMethod("DoMeetingInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MenuWhenEncounteringAnArmyPatch)
      .GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0x18, 0xC5, 0x16, 0xD2, 0x46, 0x07, 0xBB, 0x08,
        0xAE, 0xC3, 0x6D, 0xE6, 0xFA, 0xCD, 0x2E, 0x63,
        0x0E, 0x1A, 0x73, 0xB6, 0x76, 0xF9, 0xAD, 0x0E,
        0x3F, 0xD9, 0xFA, 0x7A, 0x68, 0xFB, 0xEB, 0xE7
      }
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
        CommunityPatchSubModule.Error($"{nameof(MenuWhenEncounteringAnArmyPatch)}: Could not locate all of necessary private fields for patching." + Environment.NewLine);
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
      if (!CommunityPatchSubModule.EnableMenuWhenEncouteringAnArmy)
        return true;

      var attacker = (PartyBase) EncounteredPartyField.GetValue(__instance);
      if (attacker.IsSettlement)
        foreach (var defender in MobileParty.MainParty.MapEvent.DefenderSide.Parties) {
          if (defender.IsSettlement)
            continue;

          attacker = defender;
          break;
        }

      Campaign.Current.CurrentConversationContext = ConversationContext.PartyEncounter;
      MapEventStateField.SetValue(__instance, PlayerEncounterState.Begin);
      StateHandledField.SetValue(__instance, true);

      var defenderParty = (PartyBase) DefenderPartyField.GetValue(__instance);

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