using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Antijank.Interop {

  [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 8)]
  public struct PROPVARIANT {

    [FieldOffset(0)]
    public ushort vt;

    [FieldOffset(0)]
    public decimal decValue;

    [FieldOffset(8)]
    public sbyte cVal;

    [FieldOffset(8)]
    public byte bVal;

    [FieldOffset(8)]
    public short iVal;

    [FieldOffset(8)]
    public ushort uiVal;

    [FieldOffset(8)]
    public int lVal;

    [FieldOffset(8)]
    public uint ulVal;

    [FieldOffset(8)]
    public int intVal;

    [FieldOffset(8)]
    public uint uintVal;

    [FieldOffset(8)]
    public long hVal;

    [FieldOffset(8)]
    public ulong uhVal;

    [FieldOffset(8)]
    public float fltVal;

    [FieldOffset(8)]
    public double dblVal;

    [FieldOffset(8)]
    public short boolVal;

    [MarshalAs(UnmanagedType.Error)]
    [FieldOffset(8)]
    public int scode;

    [MarshalAs(UnmanagedType.Currency)]
    [FieldOffset(8)]
    public decimal cyVal;

    [FieldOffset(8)]
    public DateTime date;

    [FieldOffset(8)]
    public FILETIME filetime;

    [FieldOffset(8)]
    public IntPtr p;

  }

}