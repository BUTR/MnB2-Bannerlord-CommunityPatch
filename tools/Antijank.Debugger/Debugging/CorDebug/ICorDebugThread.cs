using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("938C6D66-7FB6-4F69-B389-425B8987329B")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugThread {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppProcess")]
    ICorDebugProcess GetProcess();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pdwThreadId")]
    uint GetID();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("phThreadHandle")]
    SafeHandle GetHandle();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppAppDomain")]
    ICorDebugAppDomain GetAppDomain();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetDebugState([In] CorDebugThreadState state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pState")]
    CorDebugThreadState GetDebugState();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pState")]
    CorDebugUserState GetUserState();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppExceptionObject")]
    ICorDebugValue GetCurrentException();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClearCurrentException();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppStepper")]
    ICorDebugStepper CreateStepper();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppChains")]
    ICorDebugChainEnum EnumerateChains();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppChain")]
    ICorDebugChain GetActiveChain();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppFrame")]
    ICorDebugFrame GetActiveFrame();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppRegisters")]
    ICorDebugRegisterSet GetRegisterSet();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppEval")]
    ICorDebugEval CreateEval();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppObject")]
    ICorDebugValue GetObject();

  }

}