using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;
using MonoMod.Utils;
using TaleWorlds.CampaignSystem;
using static System.Reflection.Emit.OpCodes;

namespace Antijank {

  public static class MbEventExceptionHandler {

    private const BindingFlags AnyAccess = BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags Any = AnyAccess | BindingFlags.Instance | BindingFlags.Static;

    private const BindingFlags Declared = BindingFlags.DeclaredOnly | Any;

    static unsafe MbEventExceptionHandler() {
      Context.Harmony.Patch(
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

      var dynAsmName = new AssemblyName("MbEventExceptionHandlers");
      var dynAsm = AppDomain.CurrentDomain.DefineDynamicAssembly(dynAsmName, AssemblyBuilderAccess.Run);
      var dynMod = dynAsm.DefineDynamicModule(dynAsmName.Name);

      {
        var t = typeof(MbEvent<,,,,,>);
        var recType = t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).First(t => t.Name.StartsWith("EventHandlerRec"));
        var g = t.GetGenericArguments().Length;
        var dynType = dynMod.DefineType("MbEventExceptionHandler" + g, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class);
        var mi = t.GetMethod("InvokeList", Declared);
        var paramInfos = mi!.GetParameters();
        var paramTypes = new Type[paramInfos.Length + 1];
        paramTypes[0] = mi.GetThisParamType();
        for (var i = 0; i < paramInfos.Length; ++i)
          paramTypes[i + 1] = paramInfos[i].ParameterType;
        var returnType = typeof(void);
        var name = "InvokeListRedirection";
        var d = dynType.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Static);
        var gps = d.DefineGenericParameters(Enumerable.Range(0, g).Select(n => "T" + n).ToArray());
        var x = gps.Select(gp => gp.AsType()).ToArray();
        var sig = new[] {t.MakeGenericType(x), recType.MakeGenericType(x.Concat(x).ToArray())}.Concat(x).ToArray();
        d.SetParameters(sig);
        d.SetReturnType(returnType);
        InvokeListRedirectionGenerator(mi, d.GetILGenerator());
        var dt = dynType.CreateType();
        var dm = dt.GetMethod(name);
        var a = dm!.GetMethodBody()!.GetILAsByteArray();
        Console.WriteLine(BitConverter.ToString(a));
        var h = GCHandle.Alloc(a, GCHandleType.Pinned);
        var s = (ReadOnlySpan<byte>) a;
        // now we got IL ...
      }
    }

    private static void InvokeListRedirectionGenerator(MethodBase mb, ILGenerator il) {
      il.Emit(Ldarg_0); // this
      il.Emit(Ldarg_1); // list

      var paramInfos = mb.GetParameters();
      var paramLen = paramInfos.Length;
      var paramsArraySize = paramLen - 2;
      for (ushort i = 1; i < paramLen; i++) {
        switch (i) {
          case 1:
            il.Emit(Ldarg_2);
            break;
          case 2:
            il.Emit(Ldarg_3);
            break;
          default:
            il.Emit(Ldarg_S, (byte) (i + 1));
            break;
        }

        if (paramInfos[i].ParameterType.IsValueType)
          il.Emit(Box, paramInfos[i].ParameterType);
      }

      switch (paramsArraySize) {
        // @formatter:off
        case 0: il.Emit(Ldc_I4_0); break;
        case 1: il.Emit(Ldc_I4_1); break;
        case 2: il.Emit(Ldc_I4_2); break;
        case 3: il.Emit(Ldc_I4_3); break;
        case 4: il.Emit(Ldc_I4_4); break;
        case 5: il.Emit(Ldc_I4_5); break;
        case 6: il.Emit(Ldc_I4_6); break;
        case 7: il.Emit(Ldc_I4_7); break;
        case 8: il.Emit(Ldc_I4_8); break;
        // @formatter:on
        default:
          if (paramsArraySize <= 127)
            il.Emit(Ldc_I4_S, (sbyte) paramsArraySize);
          il.Emit(Ldc_I4, paramsArraySize);
          break;
      }

      il.Emit(Newarr, typeof(object));

      // call InvokeListReplacementPatchBase(MbBase<?> instance, MbBase<?>.EventHandlerRec<?> list, T# t# ...)
      il.EmitCall(Call, InvokeListReplacementBaseMethod, null);
      il.Emit(Ret);
    }

    public static void Init() {
    }

    private static readonly Dictionary<MethodInfo, bool> RetryLots
      = new Dictionary<MethodInfo, bool>();

    private static readonly Dictionary<MethodInfo, bool> AlwaysIgnore
      = new Dictionary<MethodInfo, bool>();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool InvokeListReplacementPatch(MbEvent __instance, object list)
      => InvokeListReplacementBase(__instance, list);

    public static readonly Dictionary<Type, (MethodInfo OwnerGetter, MethodInfo ActionGetter, FieldInfo NextField)> InvokeListReflectionCache
      = new Dictionary<Type, (MethodInfo, MethodInfo, FieldInfo)>();

    private static readonly MethodInfo InvokeListReplacementBaseMethod = typeof(MbEventExceptionHandler).GetMethod(nameof(InvokeListReplacementBase), Any)!;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool InvokeListReplacementBase(object instance, object eventHandlerRecList, params object[] args) {
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