using System;
using System.Runtime.CompilerServices;

namespace CommunityPatch {
  public static class FloatHelper {

    private const float Tolerance = 0.0001f;
      
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float value)
      => Math.Abs(value) < Tolerance;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDifferentFrom(this float value, float target)
      => Math.Abs(value - target) > Tolerance;

  }
}