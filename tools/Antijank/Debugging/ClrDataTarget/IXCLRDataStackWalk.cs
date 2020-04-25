using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E59D8D22-ADA7-49A2-89B5-A415AFCFC95F")]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataStackWalk {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetContext([In] uint contextFlags, [In] uint contextBufSize, out uint contextSize,
      ref byte* contextBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetContext([In] uint contextSize, [In] ref byte* context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackSizeSkipped(out ulong stackSizeSkipped);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFrameType(out CLRDataSimpleFrameType simpleType, out CLRDataDetailedFrameType detailedType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFrame([MarshalAs(UnmanagedType.Interface)] out IXCLRDataFrame frame);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetContext2([In] uint flags, [In] uint contextSize, [In] ref byte* context);

  }

}