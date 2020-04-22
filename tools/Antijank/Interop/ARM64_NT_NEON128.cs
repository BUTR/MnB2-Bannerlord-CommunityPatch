using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Interop {

  [PublicAPI]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [StructLayout(LayoutKind.Explicit, Size = 0x10)]
  public struct ARM64_NT_NEON128 {

    [FieldOffset(0)]
    public double D0;

    [FieldOffset(8)]
    public double D1;

    public unsafe ref double D(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<double>(Unsafe.AsPointer(ref D0));
        case 1: return ref Unsafe.AsRef<double>(Unsafe.AsPointer(ref D1));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

    [FieldOffset(0)]
    public float S0;

    [FieldOffset(4)]
    public float S1;

    [FieldOffset(8)]
    public float S2;

    [FieldOffset(12)]
    public float S3;

    public unsafe ref float S(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<float>(Unsafe.AsPointer(ref S0));
        case 1: return ref Unsafe.AsRef<float>(Unsafe.AsPointer(ref S1));
        case 2: return ref Unsafe.AsRef<float>(Unsafe.AsPointer(ref S2));
        case 3: return ref Unsafe.AsRef<float>(Unsafe.AsPointer(ref S3));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

    [FieldOffset(0)]
    public ulong L0;

    [FieldOffset(8)]
    public ulong L1;

    public unsafe ref ulong L(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref L0));
        case 1: return ref Unsafe.AsRef<ulong>(Unsafe.AsPointer(ref L1));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

    [FieldOffset(0)]
    public uint I0;

    [FieldOffset(4)]
    public uint I1;

    [FieldOffset(8)]
    public uint I2;

    [FieldOffset(12)]
    public uint I3;

    public unsafe ref uint I(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref I0));
        case 1: return ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref I1));
        case 2: return ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref I2));
        case 3: return ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref I3));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

    [FieldOffset(0)]
    public ushort H0;

    [FieldOffset(2)]
    public ushort H1;

    [FieldOffset(4)]
    public ushort H2;

    [FieldOffset(6)]
    public ushort H3;

    [FieldOffset(8)]
    public ushort H4;

    [FieldOffset(10)]
    public ushort H5;

    [FieldOffset(12)]
    public ushort H6;

    [FieldOffset(14)]
    public ushort H7;

    public unsafe ref ushort H(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H0));
        case 1: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H1));
        case 2: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H2));
        case 3: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H3));
        case 4: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H4));
        case 5: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H5));
        case 6: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H6));
        case 7: return ref Unsafe.AsRef<ushort>(Unsafe.AsPointer(ref H7));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

    [FieldOffset(0)]
    public byte B0;

    [FieldOffset(1)]
    public byte B1;

    [FieldOffset(2)]
    public byte B2;

    [FieldOffset(3)]
    public byte B3;

    [FieldOffset(4)]
    public byte B4;

    [FieldOffset(5)]
    public byte B5;

    [FieldOffset(6)]
    public byte B6;

    [FieldOffset(7)]
    public byte B7;

    [FieldOffset(8)]
    public byte B8;

    [FieldOffset(9)]
    public byte B9;

    [FieldOffset(10)]
    public byte B10;

    [FieldOffset(11)]
    public byte B11;

    [FieldOffset(12)]
    public byte B12;

    [FieldOffset(13)]
    public byte B13;

    [FieldOffset(14)]
    public byte B14;

    [FieldOffset(15)]
    public byte B15;

    public unsafe ref byte B(int i) {
      switch (i) {
        case 0: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B0));
        case 1: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B1));
        case 2: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B2));
        case 3: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B3));
        case 4: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B4));
        case 5: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B5));
        case 6: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B6));
        case 7: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B7));
        case 8: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B8));
        case 9: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B9));
        case 10: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B10));
        case 11: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B11));
        case 12: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B12));
        case 13: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B13));
        case 14: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B14));
        case 15: return ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref B15));
        default: throw new IndexOutOfRangeException("Index must be in range.");
      }
    }

  }

}