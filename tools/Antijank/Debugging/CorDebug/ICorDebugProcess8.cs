using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("2E6F28C1-85EB-4141-80AD-0A90944B9639")]
  [PublicAPI]
  public interface ICorDebugProcess8 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableExceptionCallbacksOutsideOfMyCode(
      [MarshalAs(UnmanagedType.Bool)] [In] bool enableExceptionsOutsideOfJMC
    );

  }

}