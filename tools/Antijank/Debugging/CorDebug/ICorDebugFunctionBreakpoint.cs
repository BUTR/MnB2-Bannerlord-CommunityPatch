using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("CC7BCAE9-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugFunctionBreakpoint : ICorDebugBreakpoint {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Activate([In] int bActive);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsActive(out int pbActive);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetOffset(out uint pnOffset);

  }

}