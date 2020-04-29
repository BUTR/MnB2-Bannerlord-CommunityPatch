using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("A5B0BEEA-EC62-4618-8012-A24FFC23934C")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataTask {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetProcess([MarshalAs(UnmanagedType.Interface)] out IXCLRDataProcess process);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentAppDomain([MarshalAs(UnmanagedType.Interface)] out IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetUniqueID(out ulong id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask task);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetManagedObject([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDesiredExecutionState(out uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetDesiredExecutionState([In] uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateStackWalk([In] uint flags, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataStackWalk stackWalk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetOSThreadID(out uint id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetContext([In] uint contextFlags, [In] uint contextBufSize, out uint contextSize,
      ref byte* contextBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetContext([In] uint contextSize, [In] ref byte* context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentExceptionState([MarshalAs(UnmanagedType.Interface)] out IXCLRDataExceptionState exception);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLastExceptionState([MarshalAs(UnmanagedType.Interface)] out IXCLRDataExceptionState exception);

  }

}