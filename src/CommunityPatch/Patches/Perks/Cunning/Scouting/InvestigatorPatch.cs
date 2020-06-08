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
