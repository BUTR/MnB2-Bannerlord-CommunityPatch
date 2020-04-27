using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCAEE-8A68-11D2-983C-0000F808342D")]
  [ComImport]
  
  public interface ICorDebugChain {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThread([MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackRange(out ulong pStart, out ulong pEnd);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetContext([MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetPrevious([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNext([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsManaged(out int pManaged);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateFrames([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrameEnum ppFrames);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetActiveFrame([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetReason(out CorDebugChainReason pReason);

  }

}