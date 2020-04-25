using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("AD1B3588-0EF0-4744-A496-AA09A9F80371")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugProcess2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadForTaskID([In] ulong taskid, [MarshalAs(UnmanagedType.Interface)] out ICorDebugThread2 ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVersion(out COR_VERSION version);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetUnmanagedBreakpoint([In] ulong address, [In] uint bufsize, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugProcess2 buffer, out uint bufLen);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClearUnmanagedBreakpoint([In] ulong address);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetDesiredNGENCompilerFlags([In] uint pdwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pdwFlags")]
    uint GetDesiredNGENCompilerFlags();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pOutValue")]
    ICorDebugReferenceValue GetReferenceValueFromGCHandle([In] UIntPtr handle);

  }

}