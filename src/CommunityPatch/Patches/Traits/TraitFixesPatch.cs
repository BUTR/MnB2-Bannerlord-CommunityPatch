using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Traits {

  public class TraitFixesPatch : PatchBase<TraitFixesPatch> {

    private static readonly Type NotableWantsDaughterFoundIssueBehaviorType
      = Type.GetType("SandBox.Quests.QuestBehaviors.NotableWantsDaughterFoundIssueBehavior, SandBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
        ?.GetNestedType("NotableWantsDaughterFoundIssueQuest", NonPublic);

    private static readonly Type CharacterCreationContentType = Type.GetType("StoryMode.CharacterCreationSystem.CharacterCreationContent, StoryMode, Version=1.0.0.0, Culture=neutral");

    private static readonly MethodInfo ApplySkillAndAttributeEffectsMethod = CharacterCreationContentType.GetMethod("ApplySkillAndAttributeEffects");

    private static readonly MethodInfo AddPlayerTraitXpMethod = typeof(TraitLevelingHelper).GetMethod("AddPlayerTraitXPAndLogEntry", NonPublic | Static);

    private static readonly MethodInfo NotableDaughterStartConvoMethod = NotableWantsDaughterFoundIssueBehaviorType.GetMethod("StartConversationAfterFight", NonPublic | Instance);

    public static readonly byte[][] ApplySkillAndAttributeEffectsHashes = {
      new byte[] {
        // e1.3.0.227640
        0x78, 0xD5, 0xD7, 0xC7, 0xA8, 0x83, 0xB5, 0x60,
        0xCD, 0x77, 0xA5, 0xC7, 0xFF, 0xE6, 0x70, 0xC8,
        0x36, 0x99, 0x61, 0x00, 0x56, 0x81, 0x90, 0xDD,
        0x14, 0xCF, 0xB6, 0x48, 0xE9, 0x0D, 0x5C, 0xEB
      }
    };

    public static readonly byte[][] AddPlayerTraitXpHashes = {
      new byte[] {
        // e1.3.0.227640
        0xE9, 0xA4, 0x2D, 0x6B, 0xE6, 0x3C, 0xD4, 0xFF,
        0x57, 0xD3, 0x5B, 0x08, 0x5B, 0x1F, 0xF1, 0x56,
        0x38, 0x0C, 0x13, 0x8C, 0x31, 0xAD, 0xBA, 0xD1,
        0xC0, 0x75, 0x77, 0x2A, 0x2F, 0x6C, 0xE3, 0x07
      }
    };

    public static readonly byte[][] NotableDaughterStartConvoHashes = {
      new byte[] {
        // e1.3.0.227640
        0xE6, 0xF3, 0x7A, 0x4F, 0xC2, 0x78, 0x1E, 0x5D,
        0x62, 0xDF, 0x01, 0xA3, 0x85, 0x99, 0x80, 0x69,
        0x41, 0xA8, 0xF3, 0x8B, 0x4B, 0xD1, 0xBF, 0x0A,
        0xBA, 0x83, 0xD4, 0x29, 0xFC, 0xF0, 0xCA, 0x21
      },
      new byte[] {
        // e1.4.0.228616
        0x3B, 0x25, 0x21, 0xEB, 0x9E, 0x77, 0x35, 0x43,
        0x5C, 0xDE, 0xCB, 0xDD, 0xDB, 0x63, 0x3E, 0xE0,
        0xB9, 0x97, 0xDD, 0xFB, 0xEB, 0x93, 0xE0, 0x3A,
        0x70, 0xDD, 0xDE, 0xCE, 0x9A, 0x50, 0xC6, 0x9D
      }
    };

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return ApplySkillAndAttributeEffectsMethod;
      yield return AddPlayerTraitXpMethod;
      yield return NotableDaughterStartConvoMethod;
    }

    private static Harmony Harmony => CommunityPatchSubModule.Harmony;

    public override bool? IsApplicable(Game game) {
      if (ApplySkillAndAttributeEffectsMethod != null
        && AddPlayerTraitXpMethod != null
        && NotableDaughterStartConvoMethod != null)
        return true;

      if (!ApplySkillAndAttributeEffectsMethod.MakeCilSignatureSha256()
        .MatchesAnySha256(ApplySkillAndAttributeEffectsHashes))
        return false;

      if (!AddPlayerTraitXpMethod.MakeCilSignatureSha256()
        .MatchesAnySha256(AddPlayerTraitXpHashes))
        return false;

      if (!NotableDaughterStartConvoMethod.MakeCilSignatureSha256()
        .MatchesAnySha256(NotableDaughterStartConvoHashes))
        return false;

      return false;
    }

    public override void Apply(Game game) {
      if (Applied)
        return;

      Harmony.Patch(ApplySkillAndAttributeEffectsMethod, postfix: new HarmonyMethod(typeof(TraitFixesPatch), nameof(ApplySkillAndAttributeEffectsPrefix)));

      Harmony.Patch(AddPlayerTraitXpMethod, postfix: new HarmonyMethod(typeof(TraitFixesPatch), nameof(AddPlayerTraitXpPrefix)));

      Harmony.Patch(NotableDaughterStartConvoMethod, transpiler: new HarmonyMethod(typeof(TraitFixesPatch), nameof(NotableDaughterStartConvoTranspiler)));
    }

    public override bool Applied { get; protected set; }

    public override void Reset() {
      // TODO: unpatch
    }

    //Patches the CharacterCreationModel to assign xp along with levels for traits so the traits are complete
    //Postfix to add on to actions and execute the AddTraitXp method
    public static void ApplySkillAndAttributeEffectsPrefix(List<SkillObject> skills, int focusToAdd, int skillLevelToAdd,
      CharacterAttributesEnum attribute, int attributeLevelToAdd,
      [CanBeNull] List<TraitObject> traits, int traitLevelToAdd, int renownToAdd, int goldToAdd) {
      if (traits == null || traitLevelToAdd <= 0 || traits.Count <= 0)
        return;

      var campaign = Campaign.Current;
      var model = campaign.Models.CharacterDevelopmentModel;
      var traitDeveloper = campaign.PlayerTraitDeveloper;

      foreach (var trait in traits) {
        //xpGate is the amount of xp needed to reach a certain level for a certain trait
        //xpValue is the amount of xp to assign. +100 to avoid accidental quest failure2 quickly removing trait
        var xpGate = model.GetTraitXpRequiredForTraitLevel(trait, traitLevelToAdd);
        var xpValue = xpGate + 100;
        traitDeveloper.AddTraitXp(trait, xpValue);
      }
    }

    //Provides color coded warnings on gaining/losing trait values
    //post fix to add on to the actions and access attributes
    public static void AddPlayerTraitXpPrefix(TraitObject trait, int xpValue, ActionNotes context, Hero referenceHero) {
      string xpMsg;
      Color color;
      if (xpValue < 0) {
        // TODO: localize
        xpMsg = $"You have lost {xpValue * -1} {trait.Name}."; //always shows positive number so you aren't losing negative values
        color = Colors.Red;
      }
      else if (xpValue > 0) {
        // TODO: localize
        xpMsg = $"You have gained {xpValue} {trait.Name}.";
        color = Colors.Green;
      }
      else {
        return;
      }

      InformationManager.DisplayMessage(new InformationMessage(xpMsg, color));
    }

    //Fixes incorrect hostile action xp value to be negative instead of positive for failing the find the daughter quest
    //Transpiler fix for a single value change
    public static IEnumerable<CodeInstruction> NotableDaughterStartConvoTranspiler(IEnumerable<CodeInstruction> instructions) {
      foreach (var instr in instructions) {
        //System.Diagnostics.Debug.WriteLine($"opcode:{instr.opcode}  operand:{instr.operand} ");
        if (instr.opcode == OpCodes.Ldc_I4_S)
          if ((sbyte) instr.operand == 50)
            instr.operand = (sbyte) -50;

        yield return instr;
      }
    }

  }

}