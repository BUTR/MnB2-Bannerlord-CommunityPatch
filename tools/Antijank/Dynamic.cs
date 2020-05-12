using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using HarmonyLib;
using MonoMod.Utils;
using Sigil;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public static class Dynamic {

    public static readonly AssemblyName AssemblyName = new AssemblyName($"{nameof(Antijank)} Dynamic Assembly");

    public static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndCollect);

    public static readonly ModuleBuilder MainModule = Assembly.DefineDynamicModule(AssemblyName.Name);

    private static readonly HashSet<string> InternalsAccessed = new HashSet<string>();

    private static long _unique = 0;

    public static TypeBuilder CreateStaticClass(string name = null)
      => MainModule.DefineType(name ?? $"StaticClass{Interlocked.Increment(ref _unique)}", TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public);

    public static void AccessInternals(MethodBase methodBase) {
      AccessInternals(methodBase.DeclaringType);
      AccessInternals(methodBase.ReflectedType);
      if (!(methodBase is MethodInfo mi))
        return;

      AccessInternals(mi.ReturnType);
      foreach (var p in mi.GetParameters())
        AccessInternals(p.ParameterType);
    }

    public static void AccessInternals(Type type)
      => AccessInternals(type.Assembly);

    private static void AccessInternals(Assembly asm) {
      lock (InternalsAccessed) {
        var asmName = asm.GetName().Name;
        if (InternalsAccessed.Add(asmName)) {
          Assembly.SetCustomAttribute(
            new CustomAttributeBuilder(
              IgnoresAccessChecksToAttribute.ConstructorInfo,
              new object[] {asmName}
            )
          );
        }
      }
    }

    public static Type CreateDelegateType(MethodInfo method, string name = null) {
      var tb = MainModule.DefineType(name ?? $"Delegate{Interlocked.Increment(ref _unique)}",
        TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public,
        typeof(MulticastDelegate));

      var ctor = tb.DefineConstructor
      (MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
        CallingConventions.Standard, new[] {typeof(object), typeof(IntPtr)});

      ctor.SetImplementationFlags(MethodImplAttributes.Runtime);

      var parameters = method.GetParameters();

      var paramLength = parameters.Length;
      if (!method.IsStatic) ++paramLength;
      var paramTypes = new Type[paramLength];

      MethodBuilder invoke;
      if (method.IsStatic) {
        for (var i = 0; i < paramLength; ++i)
          paramTypes[i] = parameters[i].ParameterType;
      }
      else {
        paramTypes[0] = method.GetThisParamType();
        for (var i = 1; i < paramLength; ++i)
          paramTypes[i] = parameters[i - 1].ParameterType;
      }

      invoke = tb.DefineMethod(
        "Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
        method.ReturnType, paramTypes);

      invoke.SetImplementationFlags(MethodImplAttributes.Runtime);

      for (var i = 0; i < parameters.Length; i++) {
        var parameter = parameters[i];
        invoke.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
      }

      return tb.CreateType();
    }

    private static readonly CustomAttributeBuilder AggressiveInlining =
      new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor(Public | Instance, null, new[] {typeof(MethodImplOptions)}, null)!, new object[] {MethodImplOptions.AggressiveInlining});


    public static TDelegate BuildInvoker<TDelegate>(this MethodBase m, bool skipVerification = true) where TDelegate : Delegate {
      var td = typeof(TDelegate);
      AccessInternals(m);
      var dtMi = td.GetMethod("Invoke", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
      var dtPs = dtMi!.GetParameters();
      var dt = Dynamic.CreateStaticClass();
      var mn = $"{m.Name}Invoker";
      var d = Emit<TDelegate>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
        allowUnverifiableCode: skipVerification, doVerify: !skipVerification);
      var ps = m.GetParameters();
      if (m.IsStatic) {
        for (ushort i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i];
          var pType = p.ParameterType;
          var dpType = dp.ParameterType;
          if (pType.IsValueType != dpType.IsValueType || !pType.IsAssignableFrom(dpType) && !dpType.IsAssignableFrom(pType))
            throw new NotImplementedException($"Unhandled parameter difference: {p.ParameterType.FullName} vs. {dp.ParameterType.FullName}");

          if (pType.IsByRef && !dpType.IsByRef)
            d.LoadArgumentAddress(i);
          else
            d.LoadArgument(i);
        }
      }
      else {
        var thisParamType = m.GetThisParamType();
        var firstParamType = dtPs[0].ParameterType;
        if (thisParamType.IsValueType != firstParamType.IsValueType || !thisParamType.IsAssignableFrom(firstParamType) && !firstParamType.IsAssignableFrom(thisParamType))
            throw new NotImplementedException($"Unhandled this parameter difference: {thisParamType} vs. {firstParamType.FullName}");

        d.LoadArgument(0);
        for (var i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i + 1];
          var pType = p.ParameterType;
          var dpType = dp.ParameterType;
          if (pType.IsValueType != dpType.IsValueType || !pType.IsAssignableFrom(dpType) && !dpType.IsAssignableFrom(pType))
            throw new NotImplementedException($"Unhandled parameter difference: {pType.FullName} vs. {dpType.FullName}");

          if (pType.IsByRef && !dpType.IsByRef)
            d.LoadArgumentAddress((ushort) (i + 1));
          else
            d.LoadArgument((ushort) (i + 1));
        }
      }

      switch (m) {
        case MethodInfo mi:
          d.Call(mi);
          break;
        case ConstructorInfo ci:
          d.Call(ci);
          break;
        default:
          throw new NotSupportedException(m.MemberType.ToString());
      }

      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
      return (TDelegate) dmi!.CreateDelegate(td);
    }


    public static Func<T, TField> BuildGetter<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = CreateStaticClass();
      var mn = $"{fieldInfo.Name}Getter";
      var d = Emit<Func<T, TField>>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
        allowUnverifiableCode: skipVerification, doVerify: !skipVerification);
      d.LoadArgument(0);
      d.LoadField(fieldInfo);

      if (fieldInfo.FieldType.IsValueType && !typeof(TField).IsValueType)
        d.Box(typeof(TField));
      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, DeclaredOnly | Static | Public);
      return (Func<T, TField>) dmi!.CreateDelegate(typeof(Func<T, TField>));
    }

    public static Action<T, TField> BuildSetter<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = CreateStaticClass();
      var mn = $"{fieldInfo.Name}Setter";
      var d = Emit<Func<T, TField>>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
        allowUnverifiableCode: skipVerification, doVerify: !skipVerification);
      d.LoadArgument(0);
      d.LoadArgument(1);
      if (!fieldInfo.FieldType.IsValueType && typeof(TField).IsValueType)
        d.Box(typeof(TField));
      d.StoreField(fieldInfo);
      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, DeclaredOnly | Static | Public);
      return (Action<T, TField>) dmi!.CreateDelegate(typeof(Action<T, TField>));
    }

    [Obsolete("Does not work yet, do not use.", true)]
    public static Func<object, object[], object> BuildDynamicInvoker(this MethodBase m, bool skipVerification = true) {
      var td = typeof(Func<object, object[], object>);
      AccessInternals(m);
      var dt = CreateStaticClass();
      var mn = $"{m.Name}Invoker";
      var d = Emit<Func<object, object[], object>>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
        allowUnverifiableCode: skipVerification, doVerify: !skipVerification);
      var ps = m.GetParameters();

      if (!m.IsStatic) {
        d.LoadArgument(0);
        var thisParamType = m.GetThisParamType();
        if (thisParamType.IsValueType)
          d.Unbox(thisParamType);
      }

      d.LoadArgument(1);
      for (ushort i = 0; i < ps.Length; i++) {
        var p = ps[i];
        if (i + 1 < ps.Length)
          d.Duplicate();
        d.LoadConstant(i);
        d.LoadElement<object>();
        if (p.ParameterType.IsValueType)
          d.UnboxAny(p.ParameterType);
      }

      switch (m) {
        case MethodInfo mi:
          d.Call(mi);
          break;
        case ConstructorInfo ci:
          d.Call(ci);
          break;
        default:
          throw new NotSupportedException(m.MemberType.ToString());
      }

      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, DeclaredOnly | Static | Public);
      return (Func<object, object[], object>) dmi!.CreateDelegate(td);
    }

    
    public static FieldRef<T, TField> BuildRef<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && typeof(T) != typeof(IntPtr) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = CreateStaticClass();
      var mn = $"{fieldInfo.Name}Getter";
      var d = Emit<FieldRef<T, TField>>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
        allowUnverifiableCode: skipVerification, doVerify: !skipVerification);
      d.LoadArgument(0);
      d.LoadFieldAddress(fieldInfo);

      if (fieldInfo.FieldType.IsValueType && !typeof(TField).IsValueType)
        d.Box(typeof(TField));
      d.Return();
      var mb = d.CreateMethod();
      mb.SetCustomAttribute(AggressiveInlining);
      var dti = dt.CreateTypeInfo();
      var dmi = dti!.GetMethod(mn, DeclaredOnly | Static | Public);
      return (FieldRef<T, TField>) dmi!.CreateDelegate(typeof(FieldRef<T, TField>));
    }
  }

}