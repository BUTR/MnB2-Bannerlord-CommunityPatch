using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("7FCC5FB5-49C0-41DE-9938-3B88B5B9ADD7")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugModule2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJMCStatus([In] int bIsJustMyCode, [In] uint cTokens, [In] ref uint pTokens);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void ApplyChanges([In] uint cbMetadata, [In] ref byte* pbMetadata, [In] uint cbIL, [In] ref byte* pbIL);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJITCompilerFlags([In] uint dwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetJITCompilerFlags(out uint pdwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ResolveAssembly([In] uint tkAssemblyRef,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugAssembly ppAssembly);

  }

}