using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("02CA073C-7079-4860-880A-C2F7A449C991")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IHostControl {

    // IHostMemoryManager
    // IHostTaskManager
    // IHostThreadPoolManager
    // IHostIoCompletionManager
    // IHostSyncManager
    // IHostAssemblyManager
    // IHostGCManager
    // IHostPolicyManager
    // IHostSecurityManager
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHostManager(
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 0)]
      out IntPtr ppObject);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetAppDomainManager([In] uint dwAppDomainID, [MarshalAs(UnmanagedType.IUnknown)] [In]
      object pUnkAppDomainManager);

  }

}