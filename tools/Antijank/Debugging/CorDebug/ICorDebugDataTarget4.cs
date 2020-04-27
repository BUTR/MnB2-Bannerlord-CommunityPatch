using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E799DC06-E099-4713-BDD9-906D3CC02CF2")]
  [ComImport]
  
  public interface ICorDebugDataTarget4 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void VirtualUnwind([In] uint threadID, [In] uint contextSize, [In] [Out] ref byte* context);

  }

}