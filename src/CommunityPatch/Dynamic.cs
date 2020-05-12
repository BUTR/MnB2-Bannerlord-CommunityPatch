using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using Sigil;

namespace CommunityPatch {

  public static class Dynamic {

    public static readonly AssemblyName AssemblyName = new AssemblyName("Community Patch Dynamic Assembly");

    public static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndCollect);

    public static readonly ModuleBuilder MainModule = Assembly.DefineDynamicModule(AssemblyName.Name);

    private static readonly HashSet<string> InternalsAccessed = new HashSet<string>();

    private static readonly CustomAttributeBuilder AggressiveInlining =
      new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] {typeof(MethodImplOptions)}, null)!, new object[] {MethodImplOptions.AggressiveInlining});

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


    public static FieldRef<T, TField> BuildRef<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && typeof(T) != typeof(IntPtr) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = Dynamic.CreateStaticClass();
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
      var dmi = dti!.GetMethod(mn, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
      return (FieldRef<T, TField>) dmi!.CreateDelegate(typeof(FieldRef<T, TField>));
    }


    public static Func<T, TField> BuildGetter<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && typeof(T) != typeof(IntPtr) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = Dynamic.CreateStaticClass();
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
      var dmi = dti!.GetMethod(mn, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
      return (Func<T, TField>) dmi!.CreateDelegate(typeof(Func<T, TField>));
    }

    public static Action<T, TField> BuildSetter<T, TField>(this FieldInfo fieldInfo, bool skipVerification = true) {
      if (fieldInfo == null)
        throw new ArgumentNullException(nameof(fieldInfo));

      if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        throw new ArgumentException("Return type does not match.");

      if (typeof(T) != typeof(object) && typeof(T) != typeof(IntPtr) && (fieldInfo.DeclaringType == null || !fieldInfo.DeclaringType.IsAssignableFrom(typeof(T))))
        throw new MissingFieldException(typeof(T).Name, fieldInfo.Name);

      var dt = Dynamic.CreateStaticClass();
      var mn = $"{fieldInfo.Name}Setter";
      var d = Emit<Action<T, TField>>.BuildStaticMethod(dt, mn, MethodAttributes.Public,
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
      var dmi = dti!.GetMethod(mn, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
      return (Action<T, TField>) dmi!.CreateDelegate(typeof(Action<T, TField>));
    }

    public static TDelegate BuildInvoker<TDelegate>(this MethodBase m) where TDelegate : Delegate {
      var td = typeof(TDelegate);
      Dynamic.AccessInternals(m);
      var dtMi = td.GetMethod("Invoke", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
      var dtPs = dtMi!.GetParameters();
      var dt = Dynamic.CreateStaticClass();
      var mn = $"{m.Name}Invoker";
      var d = Emit<TDelegate>.BuildStaticMethod(dt, mn, MethodAttributes.Public);
      var ps = m.GetParameters();
      if (m.IsStatic) {
        for (ushort i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i];
          if (p.ParameterType != dp.ParameterType)
            throw new NotImplementedException($"Unhandled parameter difference: {p.ParameterType.FullName} vs. {dp.ParameterType.FullName}");

          d.LoadArgument(i);
        }
      }
      else {
        var thisParamType = m.GetThisParamType();
        if (dtPs[0].ParameterType != thisParamType)
          throw new NotImplementedException($"Unhandled this parameter difference: {dtPs[0].ParameterType.FullName} vs. {thisParamType}");

        d.LoadArgument(0);
        for (var i = 0; i < ps.Length; i++) {
          var p = ps[i];
          var dp = dtPs[i + 1];
          if (p.ParameterType != dp.ParameterType)
            throw new NotImplementedException($"Unhandled parameter difference: {p.ParameterType.FullName} vs. {dp.ParameterType.FullName}");

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

  }

}