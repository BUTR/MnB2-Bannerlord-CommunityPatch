using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static HarmonyLib.AccessTools;

namespace CommunityPatch.Patches.Quests {

  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 4, 2)]
  public class LordWantsRivalCapturedQuestPatch : PatchBase<LordWantsRivalCapturedQuestPatch> {
    
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.4.2.231952
        0x2B, 0x96, 0x7F, 0xA2, 0x14, 0x52, 0x67, 0xA8,
        0x9C, 0xDA, 0x6E, 0x67, 0x32, 0xD7, 0x0D, 0x2D,
        0x77, 0x25, 0x08, 0x19, 0xAB, 0xCF, 0x09, 0x06,
        0xC2, 0x20, 0x8D, 0x71, 0xF2, 0x8C, 0x1F, 0x46
      }
    };
    
    private static readonly Type LordWantsRivalCapturedIssueBehaviorType =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.Issues.LordWantsRivalCapturedIssueBehavior, TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral", false);
    
    private static readonly Type LordWantsRivalCapturedIssueQuestType =
      Inner(LordWantsRivalCapturedIssueBehaviorType, "LordWantsRivalCapturedIssueQuest");
    
    private static readonly MethodInfo GetWarDeclaredQuestLog = PropertyGetter(LordWantsRivalCapturedIssueQuestType, "_warDeclaredQuestLog");
    
    public LordWantsRivalCapturedQuestPatch() {
      if (GetWarDeclaredQuestLog == null)
        return;
      
      var patchMethodInfo = typeof(LordWantsRivalCapturedQuestPatch).GetMethod(nameof(GetWarDeclaredQuestLogPrefix), all);
      PatchedMethodsInfo[GetWarDeclaredQuestLog] = new List<(string, MethodInfo)> { ("Prefix", patchMethodInfo) };
    }

    private static bool GetWarDeclaredQuestLogPrefix(QuestBase __instance, ref TextObject __result) {
      if (__instance?.QuestGiver?.MapFaction?.Name != null) {
        __result = new TextObject("{=cKz1cyuM}Your clan is now at war with {QUEST_GIVER_FACTION}. Quest is cancelled.");
        __result.SetTextVariable("QUEST_GIVER_FACTION", __instance.QuestGiver.MapFaction.Name);
        return false;
      }

      if (__instance?.QuestGiver?.Clan?.Name != null) {
        __result = new TextObject("{=cKz1cyuM}Your clan is now at war with {QUEST_GIVER_CLAN}. Quest is cancelled.");
        __result.SetTextVariable("QUEST_GIVER_CLAN", __instance.QuestGiver.Clan.Name);
        return false;
      }

      __result = new TextObject("{=cKz1cyuM}Your clan is now at war with the quest giver. Quest is cancelled.");
      return false;
    }

  }

}

