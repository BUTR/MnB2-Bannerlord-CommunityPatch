using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("7F3C24D3-7E1D-4245-AC3A-F72F8859C80C")]
  [ComConversionLoss]
  [PublicAPI]
  public interface ICorDebugGCReferenceEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugGCReferenceEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall)]
    int Next(
      [In] uint celt,
      [Out] [MarshalAs(UnmanagedType.LPArray)]
      COR_GC_REFERENCE[] segs,
      [Out] out uint pceltFetched
    );

  }

}