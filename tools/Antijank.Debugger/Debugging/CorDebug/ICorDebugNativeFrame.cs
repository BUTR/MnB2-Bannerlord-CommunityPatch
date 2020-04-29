using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("03E26314-4F76-11D3-88C6-006097945418")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugNativeFrame : ICorDebugFrame {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionToken(out uint pToken);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackRange(out ulong pStart, out ulong pEnd);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetIP(out uint pnOffset);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetIP([In] uint nOffset);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalRegisterValue([In] CorDebugRegister reg, [In] uint cbSigBlob, [In] UIntPtr pvSigBlob,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalDoubleRegisterValue([In] CorDebugRegister highWordReg, [In] CorDebugRegister lowWordReg,
      [In] uint cbSigBlob, [In] UIntPtr pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalMemoryValue([In] ulong address, [In] uint cbSigBlob, [In] UIntPtr pvSigBlob,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalRegisterMemoryValue([In] CorDebugRegister highWordReg, [In] ulong lowWordAddress, [In] uint cbSigBlob,
      [In] UIntPtr pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalMemoryRegisterValue([In] ulong highWordAddress, [In] CorDebugRegister lowWordRegister,
      [In] uint cbSigBlob, [In] UIntPtr pvSigBlob, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CanSetIP([In] uint nOffset);

  }

}