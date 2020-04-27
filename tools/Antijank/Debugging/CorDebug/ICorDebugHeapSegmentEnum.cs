using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("A2FA0F8E-D045-11DF-AC8E-CE2ADFD72085")]
  [ComConversionLoss]
  
  public interface ICorDebugHeapSegmentEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugHeapSegmentEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall)]
    int Next(
      [In] uint celt,
      [Out] [MarshalAs(UnmanagedType.LPArray)]
      COR_SEGMENT[] segs,
      [Out] out uint pceltFetched);

  }

}