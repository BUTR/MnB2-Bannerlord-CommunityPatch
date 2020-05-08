using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Antijank.Interop;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("5F696509-452F-4436-A3FE-4D11FE7E2347")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICorDebugCode2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeChunks([In] uint cbufSize, out uint pcnumChunks, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out]
      CodeChunkInfo[] chunks);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pdwFlags")]
    uint GetCompilerFlags();

  }

}