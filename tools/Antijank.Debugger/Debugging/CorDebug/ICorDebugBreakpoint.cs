using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("CC7BCAE8-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugBreakpoint {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Activate([In] int bActive);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsActive(out int pbActive);

  }

}