﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("B349ABE3-B56F-4689-BFCD-76BF39D888EA")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICLRProfiling {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AttachProfiler([In] uint dwProfileeProcessID, [In] uint dwMillisecondsMax, [In] ref Guid pClsidProfiler,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string wszProfilerPath, [In] IntPtr pvClientData, [In] uint cbClientData);

  }

}