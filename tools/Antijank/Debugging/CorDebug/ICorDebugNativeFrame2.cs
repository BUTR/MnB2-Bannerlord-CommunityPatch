using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("35389FF1-3684-4C55-A2EE-210F26C60E5E")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugNativeFrame2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsChild(out int pIsChild);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsMatchingParentFrame([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugNativeFrame2 pPotentialParentFrame, out int pIsParent);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackParameterSize(out uint pSize);

  }

}