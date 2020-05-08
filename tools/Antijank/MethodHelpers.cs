using System;
using System.Reflection;

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

  }

}