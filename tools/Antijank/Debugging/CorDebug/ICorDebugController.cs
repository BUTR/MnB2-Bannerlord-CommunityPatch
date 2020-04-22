using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("3D6F5F62-7538-11D3-8D5B-00104B35E7EF")]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugController {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Stop([In] uint dwTimeoutIgnored);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Continue([In] int fIsOutOfBand);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsRunning(out int pbRunning);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void HasQueuedCallbacks([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, out int pbQueued);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppThreads")]
    ICorDebugThreadEnum EnumerateThreads();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAllThreadsDebugState([In] CorDebugThreadState state, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pExceptThisThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Detach();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Terminate([In] uint exitCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pError")]
    ICorDebugErrorInfoEnum CanCommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugEditAndContinueSnapshot pSnapshots);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pError")]
    ICorDebugErrorInfoEnum CommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugEditAndContinueSnapshot pSnapshots);

  }

}