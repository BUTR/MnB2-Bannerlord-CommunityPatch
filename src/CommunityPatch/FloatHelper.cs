using System;
using System.Runtime.CompilerServices;

namespace CommunityPatch {

  public static class FloatHelper {
    // NOTE: AggressiveInlining is preferred here to reduce these showing up in call stacks, not so much performance

    private const float Tolerance = 0.0001f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this float value)
      => Math.Abs(value) < Tolerance;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDifferentFrom(this float value, float target)
      => Math.Abs(value - target) > Tolerance;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualOrGreaterThan(this float value, float target)
      => value.IsEqualTo(target) || value > target;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualOrLesserThan(this float value, float target)
      => value.IsEqualTo(target) || value < target;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEqualTo(this float value, float target)
      => Math.Abs(value - target) < Tolerance;

  }

}