﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;
using Antijank.Interop;

namespace Antijank.Debugging {

  [Guid("211F1254-BC7E-4AF5-B9AA-067308D83DD1")]
  [SuppressUnmanagedCodeSecurity]
  [ClassInterface(ClassInterfaceType.None)]
  [ComImport]
  [PublicAPI]
  public class EmbeddedCLRCorDebugClass : ICorDebug, EmbeddedCLRCorDebug {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Initialize();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void Terminate();

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SetManagedHandler([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugManagedCallback pCallback);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SetUnmanagedHandler([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugUnmanagedCallback pCallback);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void CreateProcess([MarshalAs(UnmanagedType.LPWStr)] [In] string lpApplicationName,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string lpCommandLine, [In] ref SECURITY_ATTRIBUTES lpProcessAttributes,
      [In] ref SECURITY_ATTRIBUTES lpThreadAttributes, [In] int bInheritHandles, [In] uint dwCreationFlags,
      [In] IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpCurrentDirectory,
      [In] STARTUPINFO lpStartupInfo, [In] PROCESS_INFORMATION lpProcessInformation,
      [In] CorDebugCreateProcessFlags debuggingFlags,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void DebugActiveProcess([In] uint id, [In] int win32Attach,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void EnumerateProcesses(
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcessEnum ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void GetProcess([In] uint dwProcessId,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void CanLaunchOrAttach([In] uint dwProcessId, [In] int win32DebuggingEnabled);

  }

}