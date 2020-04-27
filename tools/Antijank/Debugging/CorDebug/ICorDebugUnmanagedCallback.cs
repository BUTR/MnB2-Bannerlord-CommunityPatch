using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("5263E909-8CB5-11D3-BD2F-0000F80849BD")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugUnmanagedCallback {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void DebugEvent([In] UIntPtr pDebugEvent, [In] int fOutOfBand);

  }

}