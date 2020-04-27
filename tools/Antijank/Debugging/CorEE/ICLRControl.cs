using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("9065597E-D1A1-4FB2-B6BA-7E1FCE230F61")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICLRControl {

    // ICLRDebugManager
    // ICLRErrorReportingManager
    // ICLRGCManager
    // ICLRHostProtectionManager
    // ICLROnEventManager
    // ICLRPolicyManager
    // ICLRTaskManager
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCLRManager([In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 0)]
      out object ppObject);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAppDomainManagerType([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAppDomainManagerAssembly,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzAppDomainManagerType);

  }

}