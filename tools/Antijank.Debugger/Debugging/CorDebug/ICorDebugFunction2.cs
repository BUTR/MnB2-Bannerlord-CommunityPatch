using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("EF0C490B-94C3-4E4D-B629-DDC134C532D8")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorDebugFunction2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJMCStatus([In] [MarshalAs(UnmanagedType.Bool)] bool bIsJustMyCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Bool), ComAliasName("pbIsJustMyCode")]
    bool GetJMCStatus();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppCodeEnum")]
    ICorDebugCodeEnum EnumerateNativeCode();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pnVersion")]
    uint GetVersionNumber();

  }

}