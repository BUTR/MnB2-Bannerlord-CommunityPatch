using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
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

    private static readonly MethodInfo CheckMethodInfo1 = typeof(DefaultCharacterDevelopmentModel)
      .GetMethod("CalculateLearningRate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
        null, CallingConventions.Any, new[] {typeof(Hero), typeof(SkillObject), typeof(StatExplainer)}, null);

    private static readonly MethodInfo CheckMethodInfo2 = typeof(DefaultCharacterDevelopmentModel)
      .GetMethod("CalculateLearningRate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
        null, CallingConventions.Any, new[] {typeof(int), typeof(int), typeof(int), typeof(int), typeof(TextObject), typeof(StatExplainer)}, null);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(LearningRateExplainerPatch)
      .GetMethod(nameof(CalculateLearning1Limit), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(LearningRateExplainerPatch)
      .GetMethod(nameof(CalculateLearning2Limit), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return CheckMethodInfo1;
      yield return CheckMethodInfo2;
    }

    private static readonly byte[][] Hashes1 = {
      new byte[] {
        // e1.1.0.224785
        0x2A, 0x39, 0xC1, 0x7A, 0x27, 0xA0, 0x00, 0x8F,
        0x90, 0x77, 0xE8, 0xCD, 0xC9, 0xCB, 0x63, 0x13,
        0x1A, 0xD5, 0x09, 0x99, 0xDB, 0x17, 0x55, 0x5B,
        0xD6, 0x3F, 0x11, 0x5B, 0x14, 0x76, 0x8F, 0xBB
      }
    };

    private static readonly byte[][] Hashes2 = {
      new byte[] {
        // e1.1.0.224785
        0x80, 0x55, 0xAE, 0x19, 0xD4, 0xB0, 0x94, 0x5A,
        0x5C, 0x53, 0xAD, 0xFB, 0xE2, 0x2D, 0x19, 0x7E,
        0x85, 0x89, 0x10, 0xB2, 0x0C, 0x5C, 0xCA, 0x15,
        0xC7, 0x9D, 0x16, 0x08, 0xF6, 0x88, 0x9F, 0x22
      }
    };

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

        var hash = CheckMethodInfo1.MakeCilSignatureSha256();
        if (!hash.MatchesAnySha256(Hashes1))
          return false;
      }
      {
        var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo2);
        if (AlreadyPatchedByOthers(patchInfo))
          return false;

        var hash = CheckMethodInfo2.MakeCilSignatureSha256();
        if (!hash.MatchesAnySha256(Hashes2))
          return false;
      }

      return true;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CalculateLearning1Limit(Hero hero, SkillObject skill, ref StatExplainer explainer) {
      if (explainer == null)
        return;

      // frame 0 is current, 1 is patch thunk, 2 is caller 
      var sf = new StackFrame(2, false);
      if (sf.GetMethod().Name == nameof(DefaultCharacterDevelopmentModel.CalculateLearningRate))
        explainer = null;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    [MethodImpl(MethodImplOptions.NoInlining)]
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