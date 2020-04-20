using System;
using System.Runtime.CompilerServices;

namespace CommunityPatch {
  public static class FloatHelper {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float value)
      => Math.Abs(value) < 0.0001f;
  }
}