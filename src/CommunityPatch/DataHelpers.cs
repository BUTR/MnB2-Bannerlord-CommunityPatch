using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommunityPatch {

  internal static class DataHelpers {

#if DEBUG
    public static string GetFormattedCsHexArray(this byte[] bytes) {
      var sb = new StringBuilder();

      sb.AppendLine("new byte[] {");
      sb.AppendFormat("  // ").AppendLine(CommunityPatchSubModule.GameVersion.ToString());
      sb.AppendFormat($"  0x{bytes[0]:X2}");
      for (var i = 1; i < bytes.Length; ++i) {
        if (i % 8 == 0)
          sb.AppendLine(",").Append("  ");
        else
          sb.Append(", ");
        sb.AppendFormat($"0x{bytes[i]:X2}");
      }

      sb.AppendLine();
      sb.AppendLine("}");

      return sb.ToString();
    }
#endif

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
      CommunityPatchSubModule.Print("Unmatched hash: " + bytes.GetFormattedCsHexArray());
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