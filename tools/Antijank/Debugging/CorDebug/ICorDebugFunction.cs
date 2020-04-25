using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("CC7BCAF3-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugFunction {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModule([MarshalAs(UnmanagedType.Interface)] out ICorDebugModule ppModule);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClass([MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetToken(out uint pMethodDef);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNativeCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalVarSigToken(out uint pmdSig);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentVersionNumber(out uint pnCurrentVersion);

  }

}