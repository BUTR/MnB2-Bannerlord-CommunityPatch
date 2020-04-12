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
      yield return TargetMethodInfo1;
      yield return TargetMethodInfo2;
    }

    private static readonly byte[][] Hashes1 = {
      new byte[] {
        //e1.0.9
        0x74, 0x9e, 0xb0, 0xdf, 0xb1, 0x47, 0x44, 0x6a,
        0xb9, 0x95, 0x50, 0xef, 0x11, 0x36, 0xdf, 0x2f,
        0xd9, 0x86, 0x52, 0xf6, 0xa8, 0x18, 0x7b, 0x33,
        0x8f, 0x5c, 0x51, 0xd8, 0x16, 0xa5, 0xe8, 0xd6
      },
      new byte[] {
        //e1.0.10
        0x44, 0x0E, 0x3E, 0x9B, 0x5D, 0x10, 0x5E, 0xB6,
        0xDD, 0xDD, 0x07, 0xC6, 0x89, 0xE3, 0x15, 0xDC,
        0x12, 0x97, 0xFF, 0x8E, 0x24, 0x10, 0x0C, 0xC3,
        0x96, 0x88, 0xBA, 0x75, 0x4E, 0x1E, 0xC2, 0x35
      }
    };

    private static readonly byte[][] Hashes2 = {
      new byte[] {
        //e1.0.9
        0xd5, 0xd4, 0x96, 0xde, 0x83, 0x16, 0x30, 0x82,
        0x9e, 0xa7, 0x34, 0x67, 0x70, 0x7d, 0x38, 0x58,
        0x28, 0xc7, 0x32, 0xcc, 0x1f, 0x29, 0xbb, 0xe0,
        0x51, 0xeb, 0x04, 0xd1, 0x22, 0xcb, 0x2b, 0x81
      },
      new byte[] {
        //e1.0.10  
        0x2C, 0x76, 0x55, 0xB1, 0x66, 0xA6, 0x00, 0x19,
        0x4A, 0xD7, 0x84, 0x2D, 0x3E, 0x27, 0xB9, 0x68,
        0xD1, 0xBD, 0x5C, 0x3B, 0xE7, 0x94, 0x58, 0x9B,
        0x68, 0xD3, 0x3F, 0x32, 0xB4, 0x88, 0x73, 0x91
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