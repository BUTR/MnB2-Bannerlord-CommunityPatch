using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("096E81D5-ECDA-4202-83F5-C65980A9EF75")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugAppDomain2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArrayOrPointerType([In] uint elementType, [In] uint nRank, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugType pTypeArg, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionPointerType([In] uint nTypeArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugType ppTypeArgs, [MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);

  }

}