using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("B008EA8D-7AB1-43F7-BB20-FBB5A04038AE")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugClass2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppType")]
    ICorDebugType GetParameterizedType([In] uint elementType, [In] uint nTypeArgs,
      [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugType ppTypeArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJMCStatus([In] int bIsJustMyCode);

  }

}