using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class InvestigatorPatch : PerkPatchBase<InvestigatorPatch> {
    private const int SkillValuePerTrackDescriptionTier = 25; // hardcoded in DefaultMapTrackModel.GetTrackDescription()

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.3.0.228478
        0x31, 0xC4, 0xC9, 0xB4, 0x1C, 0x26, 0xEA, 0x4E,
        0x84, 0x0E, 0x6D, 0x8A, 0xF3, 0x20, 0x3F, 0xE2,
        0x6E, 0x14, 0x99, 0x22, 0xEE, 0x90, 0xFE, 0x0A,
        0x1C, 0x8E, 0xE4, 0x3B, 0x2A, 0x6B, 0x6D, 0x6D
      },
      new byte[] {
        // e1.4.1.231640
        0x27, 0x1A, 0x1E, 0xD7, 0x3E, 0x0E, 0x7E, 0x2D,
        0xD0, 0xD6, 0x6B, 0xA0, 0x0A, 0xDF, 0x9A, 0xC5,
        0x87, 0xDD, 0x03, 0x0C, 0x05, 0xBB, 0xE6, 0xA9,
        0xF6, 0xD9, 0x43, 0xC8, 0xDE, 0x8A, 0xCB, 0xC6
      }
    };

    public InvestigatorPatch() : base("IHXPw4L4") {
      var declaringType = typeof(DefaultMapTrackModel).GetNestedTypes(all).First(t => t.Name.Contains("GetTrackDescription"));
      var targetMethodInfo = declaringType.GetMethod("MoveNext", all);
      var patchMethodInfo = typeof(InvestigatorPatch).GetMethod(nameof(GetTrackDescriptionTranspiler), all);
      PatchedMethodsInfo[targetMethodInfo] = new List<(string, MethodInfo)> { ("Transpiler", patchMethodInfo) };
    }

    public static int GetSkillValueForTrackDescription(Hero hero, SkillObject scouting) {
      if (hero.GetPerkValue(ActivePatch.Perk)) {
        // 2x less skill per tier when the perk is discovered
        var baseline = (int) ActivePatch.Perk.RequiredSkillValue - SkillValuePerTrackDescriptionTier;
        return hero.GetSkillValue(scouting) * 2 - baseline;
      }
      else {
        return hero.GetSkillValue(scouting);
      }
    }

    private static IEnumerable<CodeInstruction> GetTrackDescriptionTranspiler(IEnumerable<CodeInstruction> instructions) {
      var patched = false;
      foreach (var instruction in instructions) {
        if (!patched && instruction.Calls(typeof(Hero).GetMethod("GetSkillValue", all))) {
          yield return new CodeInstruction(Call, typeof(InvestigatorPatch).GetMethod(nameof(GetSkillValueForTrackDescription)));
          patched = true;
        }
        else {
          yield return instruction;
        }
      }
    }
  }
}
