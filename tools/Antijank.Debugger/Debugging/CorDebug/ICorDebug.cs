using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

using Antijank.Interop;

namespace Antijank.Debugging {

  [Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebug {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Initialize();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Terminate();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetManagedHandler([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugManagedCallback pCallback);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetUnmanagedHandler([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugUnmanagedCallback pCallback);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateProcess([MarshalAs(UnmanagedType.LPWStr)] [In] string lpApplicationName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string lpCommandLine, [In] ref SECURITY_ATTRIBUTES lpProcessAttributes,
      [In] ref SECURITY_ATTRIBUTES lpThreadAttributes, [In] int bInheritHandles, [In] uint dwCreationFlags,
      [In] IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpCurrentDirectory,
      [In] STARTUPINFO lpStartupInfo, [In] PROCESS_INFORMATION lpProcessInformation,
      [In] CorDebugCreateProcessFlags debuggingFlags,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void DebugActiveProcess([In] uint id, [In] int win32Attach,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateProcesses([MarshalAs(UnmanagedType.Interface)] out ICorDebugProcessEnum ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetProcess([In] uint dwProcessId, [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CanLaunchOrAttach([In] uint dwProcessId, [In] int win32DebuggingEnabled);

  }

}