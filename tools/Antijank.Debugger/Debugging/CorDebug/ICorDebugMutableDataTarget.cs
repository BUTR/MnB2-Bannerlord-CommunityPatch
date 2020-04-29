using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("A1B8A756-3CB6-4CCB-979F-3DF999673A59")]
  [ComImport]
  
  public interface ICorDebugMutableDataTarget : ICorDebugDataTarget {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetPlatform(out CorDebugPlatform pTargetPlatform);
    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void ReadVirtual([In] ulong address, ref byte* pBuffer, [In] uint bytesRequested, out uint pBytesRead);
    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetThreadContext([In] uint dwThreadId, [In] uint contextFlags, [In] uint contextSize, ref byte* pContext);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void WriteVirtual([In] ulong address, [In] byte* pBuffer, [In] uint bytesRequested);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetThreadContext([In] uint dwThreadId, [In] uint contextSize, [In] byte* pContext);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ContinueStatusChanged([In] uint dwThreadId, [In] uint continueStatus);

  }

}