using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("A69ACAD8-2374-46e9-9FF8-B1F14120D296")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugHeapValue3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pAcquisitionCount")]
    uint GetThreadOwningMonitorLock([MarshalAs(UnmanagedType.Interface)] [Out]
      out ICorDebugThread ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppThreadEnum")]
    ICorDebugThreadEnum GetMonitorEventWaitList();

  }

}