using System;
using static CommunityPatch.CommunityPatchSubModule;

internal static class ModuleInitializer {

  public static void Initialize() {
    AppDomain.CurrentDomain.FirstChanceException += (sender, args) => {
      if (RecordFirstChanceExceptions)
        RecordedFirstChanceExceptions.AddLast(args.Exception);
    };
    AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
      RecordedUnhandledExceptions.AddLast((Exception) args.ExceptionObject);
    };
  }

}