using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct CLRDATA_FOLLOW_STUB_BUFFER {

    public ulong Data0;

    public ulong Data1;

    public ulong Data2;

    public ulong Data3;

    public ulong Data4;

    public ulong Data5;

    public ulong Data6;

    public ulong Data7;

    public unsafe ref ulong Data(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data0));
        case 1: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data1));
        case 2: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data2));
        case 3: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data3));
        case 4: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data4));
        case 5: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data5));
        case 6: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data6));
        case 7: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref Data7));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

  }

}