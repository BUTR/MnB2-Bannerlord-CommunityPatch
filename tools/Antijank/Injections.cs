using System;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;

namespace Antijank {

  [NonVersionable]
  public static class Injections {

    public static readonly Type Type = typeof(Injections);

    public static readonly MethodInfo Method = Type.GetMethod("Dispatch", Public | Static | DeclaredOnly);

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dispatch(int token, object data) {
      if (data is object[] args) {
        try {
          switch (token) {
            case 0:
              MbEventExceptionHandler.InvokeListReplacementBase(args[0], args[1] as object[]);
              return;
          }
          return;
        }
        catch (Exception ex) {
          Console.WriteLine($"Injection.Dispatch encountered an exception for MD 0x{token:X8}");

          Logging.Log(ex);
          throw;
        }
      }

      throw new NotImplementedException(data.GetType().AssemblyQualifiedName);
    }

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a)
      => new[] {a};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b)
      => new[] {a, b};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c)
      => new[] {a, b, c};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d)
      => new[] {a, b, c, d};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e)
      => new[] {a, b, c, d, e};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f)
      => new[] {a, b, c, d, e, f};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g)
      => new[] {a, b, c, d, e, f, g};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g, object h)
      => new[] {a, b, c, d, e, f, g, h};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g, object h, object i)
      => new[] {a, b, c, d, e, f, g, h, i};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g, object h, object i, object j)
      => new[] {a, b, c, d, e, f, g, h, i, j};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g, object h, object i, object j, object k)
      => new[] {a, b, c, d, e, f, g, h, i, j, k};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack(object a, object b, object c, object d, object e, object f, object g, object h, object i, object j, object k, object l)
      => new[] {a, b, c, d, e, f, g, h, i, j, k, l};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1>(T1 a)
      => new object[] {a};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2>(T1 a, T2 b)
      => new object[] {a, b};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3>(T1 a, T2 b, T3 c)
      => new object[] {a, b, c};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d)
      => new object[] {a, b, c, d};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5>(T1 a, T2 b, T3 c, T4 d, T5 e)
      => new object[] {a, b, c, d, e};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f)
      => new object[] {a, b, c, d, e, f};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g)
      => new object[] {a, b, c, d, e, f, g};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h)
      => new object[] {a, b, c, d, e, f, g, h};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i)
      => new object[] {a, b, c, d, e, f, g, h, i};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j)
      => new object[] {a, b, c, d, e, f, g, h, i, j};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j, T11 k)
      => new object[] {a, b, c, d, e, f, g, h, i, j, k};

    [NonVersionable]
    [TargetedPatchingOptOut("Optimization.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Pack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j, T11 k, T12 l)
      => new object[] {a, b, c, d, e, f, g, h, i, j, k, l};

  }

}