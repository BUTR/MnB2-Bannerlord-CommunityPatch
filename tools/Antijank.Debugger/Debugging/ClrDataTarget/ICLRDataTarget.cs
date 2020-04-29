using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

using Antijank.Interop;

namespace Antijank.Debugging {

  [Guid("3E11CCEE-D08B-43E5-AF01-32717A64DA03")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICLRDataTarget {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMachineType(out IMAGE_FILE_MACHINE machineType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetPointerSize(out uint pointerSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetImageBase([MarshalAs(UnmanagedType.LPWStr)] [In] string imagePath, out ulong baseAddress);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void ReadVirtual([In] ulong address, ref byte* buffer, [In] uint bytesRequested, out uint bytesRead);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void WriteVirtual([In] ulong address, [In] ref byte* buffer, [In] uint bytesRequested,
      out uint bytesWritten);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTLSValue([In] uint threadID, [In] uint index, out ulong value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTLSValue([In] uint threadID, [In] uint index, [In] ulong value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentThreadID(out uint threadID);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetThreadContext([In] uint threadID, [In] uint contextFlags, [In] uint contextSize, ref byte* context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetThreadContext([In] uint threadID, [In] uint contextSize, [In] ref byte* context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

  }

}