using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Antijank.Interop;

namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public class CLRMetaHostClass : ICLRMetaHost {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern ICLRRuntimeInfo GetRuntime(string pwzVersion, [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetVersionFromFile(string pwzFilePath, StringBuilder pwzBuffer, ref uint pcchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern IEnumUnknown EnumerateInstalledRuntimes();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern IEnumUnknown EnumerateLoadedRuntimes(IntPtr hndProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void RequestRuntimeLoadedNotification(ICLRMetaHost pCallbackFunction);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern ICLRRuntimeInfo QueryLegacyV2RuntimeBinding([In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void ExitProcess(int iExitCode);

  }

}