using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("271498C2-4085-4766-BC3A-7F8ED188A173")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataFrame {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFrameType(out CLRDataSimpleFrameType simpleType, out CLRDataDetailedFrameType detailedType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetContext([In] uint contextFlags, [In] uint contextBufSize, out uint contextSize,
      ref byte* contextBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomain([MarshalAs(UnmanagedType.Interface)] out IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumArguments(out uint numArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArgumentByIndex([In] uint index, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue arg,
      [In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumLocalVariables(out uint numLocals);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocalVariableByIndex([In] uint index, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue localVariable,
      [In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeName([In] uint flags, [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodInstance([MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumTypeArguments(out uint numTypeArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeArgumentByIndex([In] uint index,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeArg);

  }

}