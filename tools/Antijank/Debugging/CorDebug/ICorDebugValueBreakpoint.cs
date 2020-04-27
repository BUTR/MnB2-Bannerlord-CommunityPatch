using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCAEB-8A68-11D2-983C-0000F808342D")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugValueBreakpoint : ICorDebugBreakpoint {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Activate([In] int bActive);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsActive(out int pbActive);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetValue([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

  }

}