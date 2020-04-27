using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("677888B3-D160-4B8C-A73B-D79E6AAA1D13")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugMemoryBuffer {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStartAddress(out IntPtr address);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out uint pcbBufferLength);

  }

}