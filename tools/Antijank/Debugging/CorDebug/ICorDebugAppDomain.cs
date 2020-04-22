﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("3D6F5F63-7538-11D3-8D5B-00104B35E7EF")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugAppDomain : ICorDebugController {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Stop([In] uint dwTimeoutIgnored);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Continue([In] int fIsOutOfBand);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsRunning(out int pbRunning);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void HasQueuedCallbacks([MarshalAs(UnmanagedType.Interface)] [In] ICorDebugThread pThread, out int pbQueued);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateThreads([MarshalAs(UnmanagedType.Interface)] out ICorDebugThreadEnum ppThreads);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAllThreadsDebugState([In] CorDebugThreadState state, [MarshalAs(UnmanagedType.Interface)] [In] ICorDebugThread pExceptThisThread);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Detach();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Terminate([In] uint exitCode);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CanCommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In] ref ICorDebugEditAndContinueSnapshot pSnapshots, [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CommitChanges([In] uint cSnapshots, [MarshalAs(UnmanagedType.Interface)] [In] ref ICorDebugEditAndContinueSnapshot pSnapshots, [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pError);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppProcess")]
    ICorDebugProcess GetProcess();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppAssemblies")]
    ICorDebugAssemblyEnum EnumerateAssemblies();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppModule")]
    ICorDebugModule GetModuleFromMetaDataInterface(
      [MarshalAs(UnmanagedType.IUnknown)] [In]
      object pIMetaData);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppBreakpoints")]
    ICorDebugBreakpointEnum EnumerateBreakpoints();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppSteppers")]
    ICorDebugStepperEnum EnumerateSteppers();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsAttached(out int pbAttached);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint cchName,
      out uint pcchName,
      [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 0)] [Out]
      StringBuilder szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppObject")]
    ICorDebugValue GetObject();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Attach();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pId")]
    uint GetID();

  }

}