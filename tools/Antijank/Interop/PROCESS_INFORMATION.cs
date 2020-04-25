using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace Antijank.Interop {

  [PublicAPI]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [StructLayout(LayoutKind.Sequential, Pack=8)]
  public struct PROCESS_INFORMATION
  {
    public SafeProcessHandle hProcess;
    public SafeHandle hThread;
    public int dwProcessId;
    public int dwThreadId;
  }

}