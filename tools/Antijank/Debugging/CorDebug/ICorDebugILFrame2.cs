using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("5D88A994-6C30-479B-890F-BCEF88B129A5")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugILFrame2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemapFunction([In] uint newILOffset);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateTypeParameters([MarshalAs(UnmanagedType.Interface)] out ICorDebugTypeEnum ppTyParEnum);

  }

}