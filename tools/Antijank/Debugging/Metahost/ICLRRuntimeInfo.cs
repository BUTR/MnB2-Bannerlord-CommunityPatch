using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICLRRuntimeInfo {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVersionString([MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzBuffer, [In] [Out] ref uint pcchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRuntimeDirectory([MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzBuffer, [In] [Out] ref uint pcchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    int IsLoaded([In] IntPtr hndProcess);

    [LCIDConversion(3)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    void LoadErrorString([In] uint iResourceID, [MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzBuffer, [In] [Out] ref uint pcchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] [In] string pwzDllName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr)] [In] string pszProcName);

    // CorMetaDataDispenser : IMetaDataDispenser, IMetaDataDispenserEx
    // CorMetaDataDispenserRuntime : IMetaDataDispenser, IMetaDataDispenserEx
    // CorRuntimeHost : ICorRuntimeHost
    // CLRRuntimeHost : ICLRRuntimeHost
    // TypeNameFactory : ITypeNameFactory
    // CLRDebuggingLegacy : ICorDebug
    // CLRStrongName : ICLRStrongName
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 1)]
    object GetInterface([In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid rclsid, [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    int IsLoadable();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetDefaultStartupFlags([In] uint dwStartupFlags,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzHostConfigFile);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDefaultStartupFlags(out uint pdwStartupFlags, [MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzHostConfigFile, [In] [Out] ref uint pcchHostConfigFile);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void BindAsLegacyV2Runtime();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsStarted(out int pbStarted, out uint pdwStartupFlags);

  }

}