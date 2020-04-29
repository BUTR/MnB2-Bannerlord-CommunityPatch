using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("FE06DC28-49FB-4636-A4A3-E80DB4AE116C")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugDataTarget {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetPlatform(out CorDebugPlatform pTargetPlatform);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void ReadVirtual([In] ulong address, byte* pBuffer, [In] uint bytesRequested, out uint pBytesRead);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetThreadContext([In] uint dwThreadId, [In] uint contextFlags, [In] uint contextSize, byte* pContext);

  }

}