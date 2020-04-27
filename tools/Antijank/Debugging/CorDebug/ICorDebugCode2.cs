using System.Runtime.InteropServices;
using System.Security;
using Antijank.Interop;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("5F696509-452F-4436-A3FE-4D11FE7E2347")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugCode2 {

    // Token: 0x06000149 RID: 329
    void GetCodeChunks([In] uint cbufSize, out uint pcnumChunks, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out]
      CodeChunkInfo[] chunks);

    // Token: 0x0600014A RID: 330
    void GetCompilerFlags(out uint pdwFlags);

  }

}