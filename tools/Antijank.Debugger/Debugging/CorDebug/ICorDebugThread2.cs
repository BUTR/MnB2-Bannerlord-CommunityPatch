using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("2BD956D9-7B07-4BEF-8A98-12AA862417C5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugThread2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetActiveFunctions([In] uint cFunctions, out uint pcFunctions, [MarshalAs(UnmanagedType.Interface)] [In] [Out]
      ICorDebugThread2 pFunctions);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetConnectionID(out uint pdwConnectionId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pTaskId")]
    ulong GetTaskID();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVolatileOSThreadID(out uint pdwTid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void InterceptCurrentException([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFrame pFrame);

  }

}