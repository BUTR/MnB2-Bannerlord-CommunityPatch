using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("976A6278-134A-4a81-81A3-8F277943F4C3")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugBlockingObjectEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugBlockingObjectEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall)]
    int Next(
      [In] uint celt,
      [Out] [MarshalAs(UnmanagedType.LPArray)]
      CorDebugBlockingObject[] values,
      [Out] out uint pceltFetched);

  }

}