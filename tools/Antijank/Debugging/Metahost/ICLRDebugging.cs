﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("D28F3C5A-9634-4206-A509-477552EEFB10")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICLRDebugging {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OpenVirtualProcess(
      [In] ulong moduleBaseAddress,
      [MarshalAs(UnmanagedType.IUnknown)] [In]
      object pDataTarget,
      [MarshalAs(UnmanagedType.Interface)] [In]
      ICLRDebuggingLibraryProvider pLibraryProvider,
      [In] ref CLR_DEBUGGING_VERSION pMaxDebuggerSupportedVersion,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid rIidProcess,
      [MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 4)]
      out object ppProcess,
      [In] [Out] ref CLR_DEBUGGING_VERSION pVersion,
      [Out] out CLR_DEBUGGING_PROCESS_FLAGS pdwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CanUnloadNow(IntPtr hModule);

  }

}