using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("1A1F204B-1C66-4637-823F-3EE6C744A69C")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugThread4 {

    // S_OK (0) for true, S_FALSE (1) for no exception
    [MethodImpl(MethodImplOptions.InternalCall)] [PreserveSig]
    int HasUnhandledException();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppBlockingObjectEnum")]
    ICorDebugBlockingObjectEnum GetBlockingObjects();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppNotificationObject")]
    ICorDebugValue GetCurrentCustomDebuggerNotification();

  }

}