using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("8F378F6F-1017-4461-9890-ECF64C54079F")]
  [PublicAPI]
  public interface ICorDebugProcess10 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableGCNotificationEvents(
      [MarshalAs(UnmanagedType.Bool)] [In] bool fEnable
    );

  }

}