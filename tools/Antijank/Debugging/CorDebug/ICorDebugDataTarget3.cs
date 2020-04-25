using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("D05E60C3-848C-4E7D-894E-623320FF6AFA")]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugDataTarget3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLoadedModules([In] uint cRequestedModules, out uint pcFetchedModules,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugDataTarget3 pLoadedModules);

  }

}