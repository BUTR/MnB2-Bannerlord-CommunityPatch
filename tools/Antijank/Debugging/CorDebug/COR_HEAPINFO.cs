using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 20)]
  [PublicAPI]
  public struct COR_HEAPINFO {

    [FieldOffset(0)]
    [MarshalAs(UnmanagedType.Bool)]
    public bool areGCStructuresValid; // TRUE if it's ok to walk the heap, FALSE otherwise.

    [FieldOffset(4)]
    public uint pointerSize; // The size of pointers on the target architecture in bytes.

    [FieldOffset(8)]
    public uint numHeaps; // The number of logical GC heaps in the process.

    [FieldOffset(12)]
    [MarshalAs(UnmanagedType.Bool)]
    public bool concurrent; // Is the GC concurrent?

    [FieldOffset(16)]
    public CorDebugGCType gcType; // Workstation or Server?

  }

}