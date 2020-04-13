using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CommunityPatch {

  internal static class DataHelpers {

    public static readonly int VectorSizeBytes = Vector<byte>.Count;

    public static unsafe bool MatchesAnySha256(this byte[] bytes, params byte[][] others) {
      if (bytes.Length != 32)
        throw new InvalidOperationException($"Length of bytes is not 32.");

      fixed (byte* pBytes = bytes) {
        // ReSharper disable once ForCanBeConvertedToForeach
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < others.Length; i++) {
          var other = others[i];
          if (other.Length != 32)
            throw new InvalidOperationException($"Length of parameter {i} is not 32.");

          if (MatchesSha256(pBytes, other))
            return true;
        }
      }

#if DEBUG
      CommunityPatchSubModule.Print("Unmatched hash: " + BitConverter.ToString(bytes));
#endif

      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe bool MatchesSha256(byte* pBytes, byte[] other) {
      fixed (byte* pOther = other) {
        return VectorSizeBytes switch {
          // @formatter:off
          32 => *(Vector<byte>*) &pBytes[0] == *(Vector<byte>*) &pOther[0],
          16 => *(Vector<byte>*) &pBytes[0] == *(Vector<byte>*) &pOther[0]
            && *(Vector<byte>*) &pBytes[16] == *(Vector<byte>*) &pOther[16],
          _ => throw new PlatformNotSupportedException()
          // @formatter:on
        };
      }
    }

  }

}