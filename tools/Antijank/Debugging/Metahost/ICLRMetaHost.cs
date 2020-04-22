﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;
using Antijank.Interop;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICLRMetaHost {

    // ICLRRuntimeInfo
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 1)]
    ICLRRuntimeInfo GetRuntime([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzVersion,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVersionFromFile([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzFilePath,
      [MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzBuffer, [In] [Out] ref uint pcchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumUnknown EnumerateInstalledRuntimes();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumUnknown EnumerateLoadedRuntimes([In] IntPtr hndProcess);

    /* TODO: these are callback delegate pointers
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CallbackThreadSet();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CallbackThreadUnset();
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void RuntimeLoadedCallback([MarshalAs(UnmanagedType.Interface)] ICLRRuntimeInfo pRuntimeInfo, [MarshalAs(UnmanagedType.Interface)] ICLRMetaHost pfnCallbackThreadSet, [MarshalAs(UnmanagedType.Interface)] ICLRMetaHost pfnCallbackThreadUnset);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RequestRuntimeLoadedNotification(
      [MarshalAs(UnmanagedType.Interface)] [In]
      ICLRMetaHost pCallbackFunction);

    // ICLRRuntimeInfo
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 0)]
    ICLRRuntimeInfo QueryLegacyV2RuntimeBinding([In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExitProcess([In] int iExitCode);

  }

}