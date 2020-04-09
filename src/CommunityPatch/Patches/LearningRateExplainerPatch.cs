using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class LearningRateExplainerPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo1 = typeof(DefaultCharacterDevelopmentModel)
      .GetMethod("CalculateLearningLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
        null, CallingConventions.Any, new[] {typeof(Hero), typeof(SkillObject), typeof(StatExplainer)}, null);

    private static readonly MethodInfo TargetMethodInfo2 = typeof(DefaultCharacterDevelopmentModel)
      .GetMethod("CalculateLearningLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
        null, CallingConventions.Any, new[] {typeof(int), typeof(int), typeof(TextObject), typeof(StatExplainer)}, null);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(LearningRateExplainerPatch)
      .GetMethod(nameof(CalculateLearning1Limit), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(LearningRateExplainerPatch)
      .GetMethod(nameof(CalculateLearning2Limit), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo1,
        new HarmonyMethod(PatchMethodInfo1));

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo2,
        new HarmonyMethod(PatchMethodInfo2));
      Applied = true;
    }

    public bool IsApplicable(Game game) {
      {
        var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo1);
        if (AlreadyPatchedByOthers(patchInfo))
          return false;

        var bytes = TargetMethodInfo1.GetCilBytes();
        if (bytes == null) return false;

        var hash = bytes.GetSha256();
        if (!hash.SequenceEqual(new byte[] {
          0x5B, 0x47, 0x05, 0x79, 0x5A, 0x5D, 0xCB, 0xFC,
          0xFA, 0x46, 0x21, 0x96, 0x00, 0x94, 0x8D, 0xCC,
          0xC2, 0xE2, 0x35, 0x7F, 0x70, 0x0D, 0x64, 0xD5,
          0x08, 0xAA, 0x8F, 0x2B, 0xA5, 0x0B, 0xF3, 0x4B
        }))
          return false;
      }
      {
        var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo2);
        if (AlreadyPatchedByOthers(patchInfo))
          return false;

        var bytes = TargetMethodInfo2.GetCilBytes();
        if (bytes == null) return false;

        var hash = bytes.GetSha256();
        if (!hash.SequenceEqual(new byte[] {
          0x0B, 0xA1, 0x1A, 0x7D, 0x20, 0x6B, 0xC2, 0xD2,
          0x3C, 0xEA, 0xF2, 0xCB, 0x83, 0x7F, 0x88, 0x14,
          0x7D, 0xE2, 0x90, 0x71, 0x2F, 0xE0, 0x4D, 0x38,
          0xE5, 0x47, 0xCC, 0x08, 0x62, 0x1E, 0x92, 0x26
        }))
          return false;
      }

      return true;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static void CalculateLearning1Limit(Hero hero, SkillObject skill, ref StatExplainer explainer) {
      if (explainer == null)
        return;

      // frame 0 is current, 1 is patch thunk, 2 is caller 
      var sf = new StackFrame(2, false);
      if (sf.GetMethod().Name == nameof(DefaultCharacterDevelopmentModel.CalculateLearningRate))
        explainer = null;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static void CalculateLearning2Limit(int attributeValue, int focusValue, TextObject attributeName, ref StatExplainer explainer) {
      if (explainer == null)
        return;

      // frame 0 is current, 1 is patch thunk, 2 is caller
      var sf = new StackFrame(2, false);
      if (sf.GetMethod().Name == nameof(DefaultCharacterDevelopmentModel.CalculateLearningRate))
        explainer = null;
    }

    public void Reset() {
    }

  }

}