using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Antijank.Interop {

  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static class IEnumUnknownExtensions {

    public static IEnumerable<object> AsEnumerable(this IEnumUnknown enumUnk) {
      enumUnk.Reset();
      while (enumUnk.Next(1, out var unk) == 1)
        yield return unk;
    }

    public static IEnumerator<object> GetEnumerator(this IEnumUnknown enumUnk)
      => enumUnk.AsEnumerable().GetEnumerator();

  }

}