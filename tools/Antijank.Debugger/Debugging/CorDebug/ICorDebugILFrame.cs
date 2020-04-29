using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("03E26311-4F76-11D3-88C6-006097945418")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugILFrame : ICorDebugFrame {

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
    void GetIP(out uint pnOffset, out CorDebugMappingResult pMappingResult);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetIP([In] uint nOffset);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateLocalVariables([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueEnum ppValueEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalVariable([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateArguments([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueEnum ppValueEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArgument([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackDepth(out uint pDepth);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackValue([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CanSetIP([In] uint nOffset);

  }

}