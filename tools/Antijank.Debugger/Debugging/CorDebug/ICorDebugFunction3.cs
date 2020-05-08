using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("09B70F28-E465-482D-99E0-81A165EB0532")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorDebugFunction3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppReJitedILCode")]
    ICorDebugILCode GetActiveReJitRequestILCode();

  }

}