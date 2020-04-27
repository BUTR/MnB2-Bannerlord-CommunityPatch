using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("F69126B7-C787-4F6B-AE96-A569786FC670")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugVirtualUnwinder {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetContext([In] uint contextFlags, [In] uint cbContextBuf, out uint contextSize, ref byte* contextBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next();

  }

}