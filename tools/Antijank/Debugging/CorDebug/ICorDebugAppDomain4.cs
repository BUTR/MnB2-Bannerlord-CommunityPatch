using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("FB99CC40-83BE-4724-AB3B-768E796EBAC2")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugAppDomain4 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppManagedObject")]
    ICorDebugValue GetObjectForCCW(ulong ccwPointer);

  }

}