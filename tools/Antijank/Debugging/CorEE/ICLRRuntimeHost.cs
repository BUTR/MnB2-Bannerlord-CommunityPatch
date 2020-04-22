﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("90F1A06C-7712-4762-86B5-7A5EBA6BDB02")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICLRRuntimeHost {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Start();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Stop();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetHostControl([MarshalAs(UnmanagedType.Interface)] [In]
      IHostControl pHostControl);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCLRControl([MarshalAs(UnmanagedType.Interface)] out ICLRControl pCLRControl);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void UnloadAppDomain([In] uint dwAppDomainID, [In] int fWaitUntilDone);

    /* TODO: callback
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExecuteInAppDomainCallback(IntPtr cookie);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExecuteInAppDomain([In] uint dwAppDomainID, [MarshalAs(UnmanagedType.Interface)] [In]
      CLRRuntimeHost pCallback, [In] IntPtr cookie);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentAppDomainId(out uint pdwAppDomainId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExecuteApplication([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAppFullName, [In] uint dwManifestPaths,
      [MarshalAs(UnmanagedType.LPWStr)] [In] ref string ppwzManifestPaths, [In] uint dwActivationData,
      [MarshalAs(UnmanagedType.LPWStr)] [In] ref string ppwzActivationData, out int pReturnValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExecuteInDefaultAppDomain([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAssemblyPath,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzTypeName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzMethodName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzArgument, out uint pReturnValue);

  }

}