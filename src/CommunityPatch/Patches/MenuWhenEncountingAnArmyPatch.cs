using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;
using Harmony = HarmonyLib.Harmony;

namespace CommunityPatch.Patches {
  public sealed class MenuWhenEncounteringAnArmyPatch : PatchBase<MenuWhenEncounteringAnArmyPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      typeof(PlayerEncounter)
        .GetMethod("DoMeetingInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(MenuWhenEncounteringAnArmyPatch)
      .GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0x18, 0xC5, 0x16, 0xD2, 0x46, 0x07, 0xBB, 0x08,
        0xAE, 0xC3, 0x6D, 0xE6, 0xFA, 0xCD, 0x2E, 0x63,
        0x0E, 0x1A, 0x73, 0xB6, 0x76, 0xF9, 0xAD, 0x0E,
        0x3F, 0xD9, 0xFA, 0x7A, 0x68, 0xFB, 0xEB, 0xE7
      }
    };

    public override void Reset(){}

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

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }
    
    private static readonly FieldInfo EncounteredPartyField = typeof(PlayerEncounter).GetField("_encounteredParty", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly FieldInfo MapEventStateField = typeof(PlayerEncounter).GetField("_mapEventState", BindingFlags.Instance | BindingFlags.NonPublic);
    
    private static readonly FieldInfo StateHandledField = typeof(PlayerEncounter).GetField("_stateHandled", BindingFlags.Instance | BindingFlags.NonPublic);
    
    private static readonly FieldInfo DefenderPartyField = typeof(PlayerEncounter).GetField("_defenderParty", BindingFlags.Instance | BindingFlags.NonPublic);
    
    private static readonly FieldInfo MeetingDoneField = typeof(PlayerEncounter).GetField("_meetingDone", BindingFlags.Instance | BindingFlags.NonPublic);
    
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Prefix(PlayerEncounter __instance) {
      if (EncounteredPartyField == null || MapEventStateField == null || StateHandledField == null || DefenderPartyField == null || MeetingDoneField == null) {
        CommunityPatchSubModule.Error($"{typeof(MenuWhenEncounteringAnArmyPatch).Name}: Could not locate all of necessary private fields for patching." + Environment.NewLine);
        return true;
      }
      
      PartyBase party1 = (PartyBase) EncounteredPartyField.GetValue(__instance);
      if (party1.IsSettlement)
      {
        foreach (PartyBase party2 in (IEnumerable<PartyBase>) MobileParty.MainParty.MapEvent.DefenderSide.Parties)
        {
          if (!party2.IsSettlement)
          {
            party1 = party2;
            break;
          }
        }
      }
      Campaign.Current.CurrentConversationContext = ConversationContext.PartyEncounter;
      MapEventStateField.SetValue(__instance, PlayerEncounterState.Begin);
      StateHandledField.SetValue(__instance, true);

      var defenderParty = (PartyBase) DefenderPartyField.GetValue(__instance);
      
      if (PlayerEncounter.PlayerIsAttacker && defenderParty.IsMobile && defenderParty.MobileParty.Army != null && defenderParty.MobileParty.Army.LeaderParty == defenderParty.MobileParty && !defenderParty.MobileParty.Army.LeaderParty.AttachedParties.Contains(MobileParty.MainParty))
      {
        GameMenu.SwitchToMenu("army_encounter");
      }
      else
      {
        MeetingDoneField.SetValue(__instance, true);
        CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false), new ConversationCharacterData(party1.Leader, party1, false, false, false, false), "", "");
      }
      return false;
    }
  }
} 