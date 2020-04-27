using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("3151C08D-4D09-4F9B-8838-2880BF18FE51")]
  [ComImport]
  
  public interface ICLRDebuggingLibraryProvider {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ProvideLibrary(
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pWszFileName,
      [In] uint dwTimestamp,
      [In] uint dwSizeOfImage,
      out IntPtr phModule
    );

  }

}