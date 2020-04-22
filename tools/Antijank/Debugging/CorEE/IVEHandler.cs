﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("856CA1B2-7DAB-11D3-ACEC-00C04F86C309")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IVEHandler {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void VEHandler([MarshalAs(UnmanagedType.Error)] [In] int VECode, [In] VerError Context,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] [In]
      object[] psa);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetReporterFtn([In] long lFnPtr);

  }

}