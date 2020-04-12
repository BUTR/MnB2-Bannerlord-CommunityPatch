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
        0xAF, 0x91, 0x09, 0x47, 0xF6, 0x42, 0x8E, 0x17,
        0x33, 0x53, 0x2C, 0x8D, 0xF3, 0xE3, 0x16, 0x92,
        0xD3, 0xE8, 0x5C, 0xE5, 0xB1, 0xA2, 0xA6, 0x07,
        0xB4, 0x2C, 0xDC, 0x20, 0xDD, 0xC3, 0xA4, 0x77
      }
    };

    private static readonly byte[][] Hashes2 = {
      new byte[] {
        // e1.1.0.224785
        0xF7, 0x86, 0xC5, 0x7B, 0x40, 0xAC, 0x33, 0x78,
        0x9A, 0xFD, 0xFF, 0x61, 0x3E, 0xA0, 0x7D, 0x70,
        0xBC, 0x8B, 0xE3, 0x45, 0xE8, 0x73, 0x70, 0x50,
        0x86, 0x07, 0xCC, 0xE4, 0xFE, 0xAF, 0x5B, 0xC1
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