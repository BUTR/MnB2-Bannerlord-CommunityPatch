using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("3D6F5F64-7538-11D3-8D5B-00104B35E7EF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugProcess : ICorDebugController {

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void Stop([In] uint dwTimeoutIgnored);

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void Continue([In] int fIsOutOfBand);

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void IsRunning(out int pbRunning);

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void HasQueuedCallbacks([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, out int pbQueued);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppThreads")]
    new ICorDebugThreadEnum EnumerateThreads();

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void SetAllThreadsDebugState([In] CorDebugThreadState state, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pExceptThisThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void Detach();

    [MethodImpl(MethodImplOptions.InternalCall)]
    new void Terminate([In] uint exitCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pError")]
    new ICorDebugErrorInfoEnum CanCommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugEditAndContinueSnapshot pSnapshots);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pError")]
    new ICorDebugErrorInfoEnum CommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugEditAndContinueSnapshot pSnapshots);
    
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetID(out uint pdwProcessId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHandle(out SafeProcessHandle phProcessHandle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThread([In] uint dwThreadId, [MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateObjects([MarshalAs(UnmanagedType.Interface)] out ICorDebugObjectEnum ppObjects);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsTransitionStub([In] ulong address, out int pbTransitionStub);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsOSSuspended([In] uint threadID, out int pbSuspended);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadContext([In] uint threadID, [In] uint contextSize, [MarshalAs(UnmanagedType.Interface)] [In] [Out]
      ICorDebugProcess context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetThreadContext([In] uint threadID, [In] uint contextSize, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ReadMemory([In] ulong address, [In] uint size, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugProcess buffer, out UIntPtr read);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void WriteMemory([In] ulong address, [In] uint size, [In] ref byte* buffer, out UIntPtr written);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClearCurrentException([In] uint threadID);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableLogMessages([In] int fOnOff);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModifyLogSwitch([In] ref ushort pLogSwitchName, [In] int lLevel);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppAppDomains")]
    ICorDebugAppDomainEnum EnumerateAppDomains();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetObject([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppObject);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ThreadForFiberCookie([In] uint fiberCookie, [MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHelperThreadID(out uint pThreadID);

  }

}