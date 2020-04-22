using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [Guid("BACC578D-FBDD-48a4-969F-02D932B74634")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public class CLRDebuggingClass : CLRDebugging, ICLRDebugging {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void OpenVirtualProcess(ulong moduleBaseAddress, object pDataTarget,
      ICLRDebuggingLibraryProvider pLibraryProvider,
      ref CLR_DEBUGGING_VERSION pMaxDebuggerSupportedVersion, Guid rIidProcess, out object ppProcess,
      ref CLR_DEBUGGING_VERSION pVersion, out CLR_DEBUGGING_PROCESS_FLAGS pdwFlags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void CanUnloadNow(IntPtr hModule);

  }

}