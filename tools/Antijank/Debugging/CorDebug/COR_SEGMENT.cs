using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  
  public struct COR_SEGMENT {

    public ulong start; // The start address of the segment.

    public ulong end; // The end address of the segment.

    public CorDebugGenerationTypes type; // The generation of the segment.

    public uint heap; // The heap the segment resides in.

    public static readonly COR_SEGMENT Invalid
      = new COR_SEGMENT {
        start = ulong.MaxValue,
        end = ulong.MaxValue,
        type = CorDebugGenerationTypes.Invalid,
        heap = uint.MaxValue
      };

  }

}