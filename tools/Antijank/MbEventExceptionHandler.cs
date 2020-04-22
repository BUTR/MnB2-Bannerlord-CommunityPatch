using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using static System.Reflection.Emit.OpCodes;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace Antijank {

  public static class MbEventExceptionHandler {

    private const BindingFlags AnyAccess = BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags Any = AnyAccess | BindingFlags.Instance | BindingFlags.Static;

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | Any;

    static MbEventExceptionHandler() {
      AssemblyResolver.Harmony.Patch(
        typeof(MbEvent).GetMethod("InvokeList", Declared),
        new HarmonyMethod(typeof(MbEventExceptionHandler).GetMethod(nameof(InvokeListReplacementPatch), Declared)));

      var genericEventTypes = ImmutableHashSet.Create(
        typeof(MbEvent<>),
        typeof(MbEvent<,>),
        typeof(MbEvent<,,>),
        typeof(MbEvent<,,,>),
        typeof(MbEvent<,,,,>),
        typeof(MbEvent<,,,,,>)
      );

      foreach (var t in genericEventTypes) {
        AssemblyResolver.Harmony
          .Patch(
            t.GetMethod("InvokeList", Declared),
            transpiler: new HarmonyMethod(typeof(MbEventExceptionHandler), nameof(InvokeListReplacementTranspiler))
          );
      }
    }

    public static void Init() {
    }

    private static readonly Dictionary<MethodInfo, bool> RetryLots
      = new Dictionary<MethodInfo, bool>();

    private static readonly Dictionary<MethodInfo, bool> AlwaysIgnore
      = new Dictionary<MethodInfo, bool>();

    public static IEnumerable<CodeInstruction> InvokeListReplacementTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {
      yield return new CodeInstruction(Ldarg_0); // this
      yield return new CodeInstruction(Ldarg_1); // list

      var paramInfos = original.GetParameters();
      var paramLen = paramInfos.Length;
      var paramsArraySize = paramLen - 2;
      for (var i = 2; i < paramLen; i++) {
        yield return new CodeInstruction(
          i switch {
            2 => Ldarg_2, // t1
            3 => Ldarg_3, // t2
            _ => Ldarg_S // t3, t4, t5, t6
          },
          i <= 3
            ? (object) null
            : i
        );

        if (paramInfos[i].ParameterType.IsValueType)
          yield return new CodeInstruction(Box);
      }

      yield return new CodeInstruction(paramsArraySize switch {
          //0 => Ldc_I4_0, // none?
          1 => Ldc_I4_1, // t1
          2 => Ldc_I4_2, // t2
          3 => Ldc_I4_3, // t3
          4 => Ldc_I4_4, // t4
          5 => Ldc_I4_5, // t5
          6 => Ldc_I4_6, // t6
          _ => throw new NotImplementedException(paramsArraySize.ToString())
        }
      );
      yield return new CodeInstruction(Newarr, typeof(object));

      // call InvokeListReplacementPatchBase(MbBase<?> instance, MbBase<?>.EventHandlerRec<?> list, T# t# ...) 
      yield return new CodeInstruction(Call, typeof(MbEventExceptionHandler).GetMethod(nameof(InvokeListReplacementPatchBase), Any));
      yield return new CodeInstruction(Ret);
    }

    [UsedImplicitly]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool InvokeListReplacementPatch(MbEvent __instance, object list)
      => InvokeListReplacementPatchBase(__instance, list);

    public static readonly Dictionary<Type, (MethodInfo OwnerGetter, MethodInfo ActionGetter, FieldInfo NextField)> InvokeListReflectionCache
      = new Dictionary<Type, (MethodInfo, MethodInfo, FieldInfo)>();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool InvokeListReplacementPatchBase(object instance, object eventHandlerRecList, params object[] args) {
      try {
        if (eventHandlerRecList == null)
          return false;

        (MethodInfo OwnerGetter, MethodInfo ActionGetter, FieldInfo NextField) reflected;

        lock (InvokeListReflectionCache) {
          var eventHandlerRecType = eventHandlerRecList.GetType();
          if (!InvokeListReflectionCache.TryGetValue(eventHandlerRecType, out reflected)) {
            var mbEventType = instance.GetType();

            reflected = (
              OwnerGetter: mbEventType.GetMethod("get_Owner", Any),
              ActionGetter: eventHandlerRecType.GetMethod("get_Action", Any)
              ?? throw new NotImplementedException("Missing Action property somehow?"),
              NextField: eventHandlerRecType.GetField("Next", Any)
              ?? throw new NotImplementedException("Missing Next field somehow?")
            );

            InvokeListReflectionCache.Add(eventHandlerRecType, reflected);
          }
        }

        while (eventHandlerRecList != null) {
          var attempt = 0;
          for (;;) {
            ++attempt;
            var action = (Delegate) reflected.ActionGetter.Invoke(eventHandlerRecList, null);
            try {
              action.DynamicInvoke(args ?? Array.Empty<object>());
            }
            catch (Exception ex) {
              ex = CallStackHelpers.UnnestCommonExceptions(ex);
              var owner = reflected.OwnerGetter.Invoke(eventHandlerRecList, null);
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
                $"Possible Source Mod: {modInfo?.Name}\n" +
                $"Possible Source Assembly: {modAsm?.GetName().Name}\n" +
                $"{action.Method?.FullDescription()}\n" +
                $"Owner: {owner ?? "(missing)"}\n" +
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

          eventHandlerRecList = reflected.NextField.GetValue(eventHandlerRecList);
        }

        return false;
      }
      catch {
        Debugger.Break();
        throw;
      }
    }

  }

}