using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("75DA9E4C-BD33-43C8-8F5C-96E8A5241F57")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataExceptionState {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetPrevious([MarshalAs(UnmanagedType.Interface)] out IXCLRDataExceptionState exState);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetManagedObject([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetBaseType(out CLRDataBaseExceptionType type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCode(out uint code);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetString([In] uint bufLen, out uint strLen, out ushort str);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void IsSameState([In] ref EXCEPTION_RECORD64 exRecord, [In] uint contextSize, [In] ref byte* cxRecord);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void IsSameState2([In] uint flags, [In] ref EXCEPTION_RECORD64 exRecord, [In] uint contextSize,
      [In] ref byte* cxRecord);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTask([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTask task);

  }

}