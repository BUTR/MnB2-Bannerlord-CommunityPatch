using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("76D7DAB8-D044-11DF-9A15-7E29DFD72085")]
  [ComConversionLoss]
  [PublicAPI]
  public interface ICorDebugHeapEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugHeapEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall)]
    int Next(
      [In] uint celt,
      [Out] [MarshalAs(UnmanagedType.LPArray)]
      COR_HEAPOBJECT[] objs,
      [Out] out uint pceltFetched);

  }

}