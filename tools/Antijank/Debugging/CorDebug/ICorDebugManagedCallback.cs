﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("3D6F5F60-7538-11D3-8D5B-00104B35E7EF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugManagedCallback {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Breakpoint([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugBreakpoint pBreakpoint);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StepComplete([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugStepper pStepper, [In] CorDebugStepReason reason);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Break([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread thread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Exception([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [In] int unhandled);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EvalComplete([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugEval pEval);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EvalException([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugEval pEval);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateProcess([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExitProcess([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateThread([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread thread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExitThread([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread thread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LoadModule([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugModule pModule);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void UnloadModule([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugModule pModule);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LoadClass([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass c);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void UnloadClass([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass c);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void DebuggerError([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess, [MarshalAs(UnmanagedType.Error)] [In] int errorHR, [In] uint errorCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LogMessage([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [In] int lLevel, [In] ref ushort pLogSwitchName, [In] ref ushort pMessage);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LogSwitch([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [In] int lLevel, [In] uint ulReason, [In] ref ushort pLogSwitchName,
      [In] ref ushort pParentName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateAppDomain([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExitAppDomain([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LoadAssembly([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAssembly pAssembly);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void UnloadAssembly([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAssembly pAssembly);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ControlCTrap([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugProcess pProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NameChange([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void UpdateModuleSymbols([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugModule pModule, [MarshalAs(UnmanagedType.Interface)] [In]
      IStream pSymbolStream);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EditAndContinueRemap([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFunction pFunction, [In] int fAccurate);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void BreakpointSetError([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugAppDomain pAppDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugThread pThread, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugBreakpoint pBreakpoint, [In] uint dwError);

  }

}