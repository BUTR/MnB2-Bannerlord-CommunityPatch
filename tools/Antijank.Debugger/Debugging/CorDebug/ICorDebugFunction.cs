using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [Guid("CC7BCAF3-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorDebugFunction {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppModule")]
    ICorDebugModule GetModule();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppClass")]
    ICorDebugClass GetClass();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetToken(out uint pMethodDef);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppCode")]
    ICorDebugCode GetILCode();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppCode")]
    ICorDebug GetNativeCode();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppBreakpoint")]
    ICorDebugFunctionBreakpoint CreateBreakpoint();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pmdSig")]
    uint GetLocalVarSigToken();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pnCurrentVersion")]
    uint GetCurrentVersionNumber();

  }

}