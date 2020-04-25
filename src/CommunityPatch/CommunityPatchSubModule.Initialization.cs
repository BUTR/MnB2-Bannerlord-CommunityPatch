using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.SaveSystem.Definition;

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