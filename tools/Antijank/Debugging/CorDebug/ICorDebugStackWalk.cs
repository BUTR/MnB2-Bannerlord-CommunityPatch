using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("A0647DE9-55DE-4816-929C-385271C64CF7")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugStackWalk {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetContext([In] uint contextFlags, [In] uint contextBufSize, out uint contextSize,
      ref byte* contextBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetContext([In] CorDebugSetContextFlag flag, [In] uint contextSize, [In] ref byte* context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFrame([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame pFrame);

  }

}