using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;
using Sigil;
using Sigil.NonGeneric;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem.Definition;
using static CommunityPatch.CommunityPatchSubModule;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    private const BindingFlags AnyDeclared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    static CommunityPatchSubModule() {
      CallStackHelpers.Init();
      // catch and record exceptions
      AppDomain.CurrentDomain.FirstChanceException += (sender, args) => {
        if (RecordFirstChanceExceptions)
          RecordedFirstChanceExceptions.AddLast(args.Exception);
      };
      AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
        RecordedUnhandledExceptions.AddLast((Exception) args.ExceptionObject);
        if (args.IsTerminating)
          Diagnostics.GenerateReport();
        else
          Diagnostics.QueueGenerateReport();
      };

      try {
        Harmony.Patch(typeof(DefinitionContext)
            .GetMethod("TryGetTypeDefinition", AnyDeclared),
          postfix: new HarmonyMethod(typeof(CommunityPatchSubModule).GetMethod(nameof(TryGetTypeDefinitionPatch), AnyDeclared))
        );
      }
      catch (Exception ex) {
        Error(ex, "Couldn't apply application tick exception catcher patch.");
      }

      // 
    }

    private static void TryGetTypeDefinitionPatch(ref DefinitionContext __instance, ref object __result, ref object saveId) {
      if (__result != null)
        return;
    }

  }

}