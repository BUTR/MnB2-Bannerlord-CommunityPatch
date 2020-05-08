using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using InlineIL;
using Sigil;

namespace Antijank {

  internal static class MethodHelpers {

    public static Type GetThisParamType(this MethodBase method) {
      if (method.IsStatic)
        return null;

      var type = method.DeclaringType;
      if (type!.IsValueType)
        type = type.MakeByRefType();
      return type;
    }

    private static readonly Dictionary<MethodBase, IntPtr> MethodPointers
      = new Dictionary<MethodBase, IntPtr>();

    public static IntPtr GetPointer(this MethodInfo method, params Type[] genericArgs) {
      if (method.IsVirtual)
        throw new NotImplementedException();

      if (MethodPointers.TryGetValue(method, out var p))
        return p;

      Dynamic.AccessInternals(method);
      var tb = Dynamic.CreateStaticClass();

      var dm = tb.DefineMethod("Ldftn", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(IntPtr), Type.EmptyTypes);
      if (method.ContainsGenericParameters) {
        if (genericArgs == null || genericArgs.Length < 1)
          throw new InvalidOperationException("A pointer to a method with generic parameters can only be created if generic arguments are supplied.");

        var gpnSet = new HashSet<string>();
        var gpnList = new List<string>();
        foreach (var mp in method.GetParameters()) {
          var pt = mp.ParameterType;
          if (pt.IsGenericParameter) {
            gpnSet.Add(pt.Name);
            continue;
          }

          if (!pt.ContainsGenericParameters)
            continue;

          foreach (var ga in pt.GetGenericArguments())
            if (gpnSet.Add(ga.Name))
              gpnList.Add(ga.Name);
        }

        if (gpnSet.Count > genericArgs.Length)
          throw new InvalidOperationException("Not enough generic arguments to fill generic parameters list.");

        dm.DefineGenericParameters(gpnList.ToArray());
      }

      var ilg = dm.GetILGenerator();
      ilg.Emit(OpCodes.Ldftn, method);
      ilg.Emit(OpCodes.Ret);
      var t = tb.CreateType();
      var m = t.GetMethod("Ldftn");
      if (genericArgs != null && genericArgs.Length > 0)
        m = m!.MakeGenericMethod(genericArgs);
      var d = (Func<IntPtr>) m!.CreateDelegate(typeof(Func<IntPtr>));
      p = d();
      MethodPointers.Add(method, p);

      return p;
    }

  }

}