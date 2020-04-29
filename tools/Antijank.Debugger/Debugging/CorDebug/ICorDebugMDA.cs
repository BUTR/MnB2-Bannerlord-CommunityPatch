using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC726F2F-1DB7-459B-B0EC-05F01D841B42")]
  [ComImport]
  
  public interface ICorDebugMDA {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint cchName, out uint pcchName, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugMDA szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDescription([In] uint cchName, out uint pcchName, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugMDA szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetXML([In] uint cchName, out uint pcchName, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugMDA szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags([In] ref CorDebugMDAFlags pFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetOSThreadId(out uint pOsTid);

  }

}