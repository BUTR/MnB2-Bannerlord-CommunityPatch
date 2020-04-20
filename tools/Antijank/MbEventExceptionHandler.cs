using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Antijank {

  public static class MbEventExceptionHandler {

    private const BindingFlags AnyAccess = BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags Any = AnyAccess | BindingFlags.Instance | BindingFlags.Static;

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | Any;

    static MbEventExceptionHandler()
      => AssemblyResolver.Harmony.Patch(
        typeof(MbEventExceptionHandler).GetMethod("LoadSubModules", Declared),
        new HarmonyMethod(typeof(MbEventExceptionHandler).GetMethod(nameof(InvokeListReplacementPatch), Declared)));

    public static void Init() {
    }

    private static readonly Type EventHandlerRecType = typeof(MbEvent).GetNestedType("EventHandlerRec", Declared);

    private static readonly MethodInfo EventHandlerRecGetAction = EventHandlerRecType.GetMethod("get_Action", Any);

    private static readonly MethodInfo EventHandlerRecGetOwner = EventHandlerRecType.GetMethod("get_Owner", Any);

    private static readonly FieldInfo EventHandlerRecNext = EventHandlerRecType.GetField("Next", Any);

    private static readonly Dictionary<MethodInfo, bool> RetryLots
      = new Dictionary<MethodInfo, bool>();

    private static readonly Dictionary<MethodInfo, bool> AlwaysIgnore
      = new Dictionary<MethodInfo, bool>();

    public static bool InvokeListReplacementPatch(MbEvent __instance, object list) {
      while (list != null) {
        var attempt = 0;
        for (;;) {
          ++attempt;
          var action = (Action) EventHandlerRecGetAction.Invoke(list, null);
          try {
            action();
          }
          catch (Exception ex) {
            var owner = EventHandlerRecGetOwner.Invoke(list, null);
            if (RetryLots.TryGetValue(action.Method, out var retryLots)) {
              if (retryLots)
                if (attempt % 100 != 0)
                  continue;
            }

            var answeredAlwaysIgnore = AlwaysIgnore.TryGetValue(action.Method, out var alwaysIgnore);
            if (answeredAlwaysIgnore) {
              if (alwaysIgnore)
                break;
            }

            var modAsm = new StackTrace(ex, false).FindModuleFromStackTrace(out var modInfo);

            void ShowDetails()
              => MessageBox.Info(ex.ToString(), "Exception Details");
            
            switch (MessageBox.Error(
              $"Exception processing game event handler, attempt #{attempt}.\n" +
              $"Possible Source Mod: {modInfo.Name}\n" +
              $"Possible Source Assembly: {modAsm.GetName().Name}\n" +
              $"{action.Method.FullDescription()}\n" +
              $"Owner: {owner}\n" +
              $"Exception: {ex.GetType().Name}: {ex.Message}",
              type: MessageBoxType.AbortRetryIgnore,
              help: ShowDetails)) {
              default:
                throw;

              case MessageBoxResult.Retry: {
                RetryLots[action.Method] = MessageBox.Error(
                  "Remember this decision? Will retry 100 more times before asking again.",
                  type: MessageBoxType.YesNo) == MessageBoxResult.Yes;
                continue;
              }

              case MessageBoxResult.Ignore: {
                if (answeredAlwaysIgnore)
                  break;

                AlwaysIgnore[action.Method] = MessageBox.Error(
                  "Remember this decision? Will not ask again for this caller.",
                  type: MessageBoxType.YesNo) == MessageBoxResult.Yes;
                break;
              }
            }
          }

          break;
        }

        list = EventHandlerRecNext.GetValue(list);
      }

      return false;
    }

  }

}