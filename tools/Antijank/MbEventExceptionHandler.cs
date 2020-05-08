using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Antijank.Debugging;
using Antijank.Interop;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;
using static System.Reflection.Emit.OpCodes;
using Debugger = System.Diagnostics.Debugger;
using MethodAttributes = System.Reflection.MethodAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace Antijank {

  public static class MbEventExceptionHandler {

    private const BindingFlags AnyAccess = Public | NonPublic;

    private const BindingFlags Any = AnyAccess | Instance | Static;

    private const BindingFlags Declared = DeclaredOnly | Any;

    public static readonly Dictionary<Type, InvokeListReflectionCacheImpl> InvokeListReflectionCache = new Dictionary<Type, InvokeListReflectionCacheImpl>();

    public static readonly MethodInfo InvokeListReplacementBaseMethod = typeof(MbEventExceptionHandler).GetMethod(nameof(InvokeListReplacementBase), Any)!;

    private static int InitCount = 0;

    static unsafe MbEventExceptionHandler() {
      RuntimeHelpers.PrepareMethod(InvokeListReplacementBaseMethod.MethodHandle);
      if (InitCount > 0) {
        if (Debugger.IsAttached)
          Debugger.Break();
        throw new InvalidOperationException("Multiple static initializer runs!");
      }

      ++InitCount;
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

      var thisAsm = typeof(MbEventExceptionHandler).Assembly;
      var thisAsmName = thisAsm.GetName();
      var thisAsmVersion = thisAsmName.Version;

      var dynAsmName = new AssemblyName("MbEventExceptionHandlers");
      var dynAsm = AppDomain.CurrentDomain.DefineDynamicAssembly(dynAsmName, AssemblyBuilderAccess.Run);
      var dynMod = dynAsm.DefineDynamicModule(dynAsmName.Name);

      var tAsm = genericEventTypes.First().Assembly;
      var tMod = tAsm.ManifestModule;
      var modName = Path.GetFileName(new Uri(tAsm.CodeBase).LocalPath);
      if (!DebuggerContext.GetInjections(modName, out var injections))
        throw new NotImplementedException($"No injection present in {modName}!");

      var objType = typeof(object);
      var mdTypeRefObj = 0;
      for (var i = 1; i < 0xFFFFFF; ++i) {
        var token = 0x01000000 | i;
        Type typeRef;
        try {
          typeRef = tMod.ResolveType(token);
        }
        catch {
          break;
        }

        if (typeRef != objType)
          continue;

        mdTypeRefObj = token;
        break;
      }

      if (mdTypeRefObj == 0)
        throw new NotImplementedException("Can't find object type reference in assembly metadata.");

      var asmPath = Path.GetFullPath(new Uri(tAsm.CodeBase).LocalPath);

      ICorDebugModule module = null;
      uint newMdToken;
      DebuggerContext.Current.Send(() => {
        module = DebuggerContext.FindModule(asmPath);
        if (module == null)
          throw new NotImplementedException();
      });

      foreach (var t in genericEventTypes) {
        var mi = t.GetMethod("InvokeList", Declared);
        byte* codeAddr = null;
        uint codeSize = 0;
        DebuggerContext.Current.Send(() => {
          var func = module.GetFunctionFromToken((uint) mi!.MetadataToken);
          var ilCode = func.GetILCode();
          codeAddr = (byte*) ilCode.GetAddress();
          codeSize = ilCode.GetSize();
          //ilCode.CreateBreakpoint(0);
        });

        var mbrLive = MethodBodyReader.Create(codeAddr, (int) codeSize);
        Console.WriteLine($"InvokeList @ 0x{((UIntPtr) codeAddr).ToUInt64():X16}:");
        ConsoleWriteIl(mbrLive);

        var recType = t.GetNestedTypes(Public | NonPublic).First(t => t.Name.StartsWith("EventHandlerRec"));
        var g = t.GetGenericArguments().Length;
        var dynType = dynMod.DefineType("MbEventExceptionHandler" + g, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class);
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
        InvokeListRedirectionGenerator(mi, d.GetILGenerator(), codeSize);
        var dt = dynType.CreateType();
        var dm = dt.GetMethod(name);
        var a = dm!.GetMethodBody()!.GetILAsByteArray();
        Console.WriteLine(BitConverter.ToString(a));
        Console.WriteLine("InvokeListRedirection:");
        var mbrRepl = MethodBodyReader.Create(a, a.Length);
        ConsoleWriteIl(mbrRepl);

        /*
        var boxIndex = Array.IndexOf(a, (byte) Box.Value);
        var boxVarIndex = 0;
        do {
          Unsafe.WriteUnaligned(ref a[boxIndex + 1], injections->GetGenVar(boxVarIndex++));
          boxIndex = Array.IndexOf(a, (byte) Box.Value, boxIndex + 1);
        } while (boxIndex != -1);
        */

        // skip first arg
        var packIndex = paramInfos.Length - 2;
        if (packIndex < 0)
          throw new NotImplementedException();

        var callIndex = Array.IndexOf(a, (byte) Call.Value);
        if (callIndex == -1)
          throw new NotImplementedException("Can't find first call instruction offset.");
        if (a[callIndex + 4] != 10)
          throw new NotImplementedException("Found first call instruction offset but wasn't followed by a member ref.");

        Unsafe.WriteUnaligned(ref a[callIndex + 1], injections->GetGenPackSpec(packIndex));

        callIndex = Array.IndexOf(a, (byte) Call.Value, callIndex + 1);
        if (callIndex == -1)
          throw new NotImplementedException("Can't find second call instruction offset.");
        if (a[callIndex + 4] != 10)
          throw new NotImplementedException("Found second call instruction offset but wasn't followed by a member ref.");

        Unsafe.WriteUnaligned(ref a[callIndex + 1], injections->Pack2);

        callIndex = Array.IndexOf(a, (byte) Call.Value, callIndex + 1);
        if (callIndex == -1)
          throw new NotImplementedException("Can't find second call instruction offset.");
        if (a[callIndex + 4] != 10)
          throw new NotImplementedException("Found second call instruction offset but wasn't followed by a member ref.");

        Unsafe.WriteUnaligned(ref a[callIndex + 1], injections->Dispatch);

        WindowsMemory.WithUnprotectedRegion((IntPtr) codeAddr, codeSize, () => {
          fixed (byte* p = a)
            Unsafe.CopyBlockUnaligned(codeAddr, p, codeSize);
        });

        Console.WriteLine($"InvokeList @ 0x{((UIntPtr) codeAddr).ToUInt64():X16} Replaced:");
        ConsoleWriteIl(mbrLive);
      }
    }

    private static void ConsoleWriteIl(MethodBodyReader mbr) {
      var offset = mbr.Offset;
      foreach (var instr in mbr) {
        Console.Write($"IL_{offset:X4}: ");
        var operand = instr.Operand;
        operand = operand switch {
          sbyte v => $"0x{v:X2}",
          byte v => $"0x{v:X2}u",
          short v => $"0x{v:X4}",
          ushort v => $"0x{v:X4}u",
          int v => $"0x{v:X8}",
          uint v => $"0x{v:X8}u",
          long v => $"0x{v:X16}",
          ulong v => $"0x{v:X16}u",
          float v => $"{v:G9}f",
          double v => $"{v:G17}d",
          int[] av => string.Join(",",
            $"0x{av.Select(v => $"{v:X8}")}"),
          _ => operand
        };
        Console.WriteLine($"{instr.Code} {operand}");
        offset = mbr.Offset;
      }
    }

    private static void InvokeListRedirectionGenerator(MethodInfo mi, ILGenerator il, uint codeSize) {
      il.Emit(Ldc_I4_0); // dispatch index 0

      il.Emit(Ldarg_0); // this

      //il.Emit(Ldarg_1); // this._nonSerializedListenerList

      var paramInfos = mi.GetParameters();
      var paramLen = paramInfos.Length;
      for (ushort i = 2; i <= paramLen; i++) {
        switch (i) {
          case 2:
            il.Emit(Ldarg_2);
            break;
          case 3:
            il.Emit(Ldarg_3);
            break;
          default:
            il.Emit(Ldarg_S, (byte) i);
            break;
        }

        //il.Emit(Box, 0x1B000000 | i);
        /*
        var paramType = paramInfos[i].ParameterType;
        if (paramType.IsValueType)
          il.Emit(Box, paramType);
        */
      }

      il.Emit(Call, 0x0A000000); // pack method call 1
      il.Emit(Call, 0x0A000000); // pack method call 2

      il.EmitCall(Call, Injections.Method, null);

      while (il.ILOffset < codeSize - 1)
        il.Emit(Nop);

      il.Emit(Ret);
      if (il.ILOffset != codeSize)
        throw new InvalidOperationException($"The IL doesn't fit: {il.ILOffset} vs. {codeSize}");
    }

    private static void EmitConst(int v, ILGenerator il) {
      switch (v) {
        // @formatter:off
        //case 0: il.Emit(Ldc_I4_0); break;
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
          if (v <= 127)
            il.Emit(Ldc_I4_S, (sbyte) v);
          il.Emit(Ldc_I4, v);
          break;
      }
    }

    public static void Init() {
    }

    private static readonly Dictionary<MethodInfo, bool> RetryLots
      = new Dictionary<MethodInfo, bool>();

    private static readonly Dictionary<MethodInfo, bool> AlwaysIgnore
      = new Dictionary<MethodInfo, bool>();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool InvokeListReplacementPatch(MbEvent __instance, object list) {
      InvokeListReplacementBase(__instance);
      return false;
    }

    public class InvokeListReflectionCacheImpl {

      public readonly Func<object, object> RecListGetter;

      public readonly Func<object, object> OwnerGetter;

      public readonly Func<object, object> ActionGetter;

      public readonly Func<object, object> NextGetter;

      public readonly Dictionary<MethodBase, Func<object, object[], object>> ActionInvokerCache;

      public InvokeListReflectionCacheImpl(Func<object, object> recListGetter,
        Func<object, object> ownerGetter,
        Func<object, object> actionGetter,
        Func<object, object> nextGetter,
        Dictionary<MethodBase, Func<object, object[], object>> actionInvokerCache) {
        RecListGetter = recListGetter;
        OwnerGetter = ownerGetter;
        ActionGetter = actionGetter;
        NextGetter = nextGetter;
        ActionInvokerCache = actionInvokerCache;
      }

    }

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type FastGetType(object instance)
      => instance.GetType();

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeListReplacementBase(object instance, params object[] args) {
      try {
        var mbEventType = FastGetType(instance);

        InvokeListReflectionCacheImpl r;

        lock (InvokeListReflectionCache) {
          if (!InvokeListReflectionCache.TryGetValue(mbEventType, out r)) {
            var eventHandlerRecListField = mbEventType.GetField("_nonSerializedListenerList", Any);
            var eventHandlerRecType = eventHandlerRecListField!.FieldType;
            var ownerGetterMethod = eventHandlerRecType.GetMethod("get_Owner", Any);
            var actionGetterMethod = eventHandlerRecType.GetMethod("get_Action", Any);
            var nextField = eventHandlerRecType.GetField("Next", Any);

            //var refEventHandlerRecListField = AccessTools.FieldRefAccess<object, object>(eventHandlerRecListField);
            //var refNextField = AccessTools.FieldRefAccess<object, object>(nextField);

            r = new InvokeListReflectionCacheImpl(
              //target => eventHandlerRecListField.GetValue(target),
              //target => refEventHandlerRecListField(target),
              eventHandlerRecListField!.BuildGetter<object, object>(),
              //target => ownerGetterMethod!.Invoke(target, null),
              ownerGetterMethod!.BuildInvoker<Func<object, object>>(),
              //target => actionGetterMethod!.Invoke(target, null),
              actionGetterMethod!.BuildInvoker<Func<object, object>>(),
              //target => nextField!.GetValue(target)
              //target => refNextField(target)
              nextField!.BuildGetter<object, object>(),
              new Dictionary<MethodBase, Func<object, object[], object>>()
            );

            InvokeListReflectionCache.Add(mbEventType, r);
          }
        }

        var eventHandlerRecList = r.RecListGetter(instance);
        if (eventHandlerRecList == null)
          return;

        var actions = ((MulticastDelegate) r.ActionGetter(eventHandlerRecList)).GetInvocationList();
        var actionInvokerCache = r.ActionInvokerCache;
        foreach (var action in actions)
          do {
            var attempt = 0;
            for (;;) {
              ++attempt;
              Func<object, object[], object> actionInvoker;
              lock (actionInvokerCache) {
                if (!actionInvokerCache.TryGetValue(action.Method, out actionInvoker)) {
                  var handler = MethodInvoker.GetHandler(action.Method, true);
                  actionInvoker = (target, args2) => handler(target, args2);
                  actionInvokerCache.Add(action.Method, actionInvoker);
                }
              }

              try {
                actionInvoker(action.Target, args);
              }
              catch (Exception ex) {
                ex = CallStackHelpers.UnnestCommonExceptions(ex);
                var owner = r.OwnerGetter(eventHandlerRecList);
                if (RetryLots.TryGetValue(action.Method, out var retryLots)) {
                  if (retryLots)
                    if (attempt % 100 != 0)
                      continue;
                }

                var answeredAlwaysIgnore = AlwaysIgnore.TryGetValue(action.Method, out var alwaysIgnore);
                if (answeredAlwaysIgnore) {
                  if (alwaysIgnore)
                    continue;
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
                    throw ex;

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

            eventHandlerRecList = r.NextGetter(eventHandlerRecList);
          } while (eventHandlerRecList != null);

        // fin
      }
      catch {
        // damn
        if (Debugger.IsAttached)
          Debugger.Break();
        throw;
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void InvokeHandler(MulticastDelegate action, object[] args) {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      RuntimeHelpers.PrepareConstrainedRegions();
      RuntimeHelpers.PrepareMethod(action.Method.MethodHandle);
      try { }
      finally {
        MethodInvoker.GetHandler(action.Method).Invoke(action.Target, args);
      }
    }

  }

}