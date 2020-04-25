using System;
using System.Collections.Generic;

namespace Antijank {

  internal static class TypeHelpers {

    public static IEnumerable<Type> GetAllNestedTypes(this Type t) {
      foreach (var nt in t.GetNestedTypes()) {
        yield return nt;

        foreach (var snt in GetAllNestedTypes(nt))
          yield return snt;
      }
    }

    public static IEnumerable<Type> WithAllNestedTypes(this IEnumerable<Type> types) {
      foreach (var t in types) {
        yield return t;

        foreach (var nt in t.GetNestedTypes()) {
          yield return nt;

          foreach (var snt in GetAllNestedTypes(nt))
            yield return snt;
        }
      }
    }

  }

}