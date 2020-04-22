﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("EF0C490B-94C3-4E4D-B629-DDC134C532D8")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugFunction2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetJMCStatus([In] int bIsJustMyCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetJMCStatus(out int pbIsJustMyCode);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateNativeCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCodeEnum ppCodeEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVersionNumber(out uint pnVersion);

  }

}