using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("e6e91d79-693d-48bc-b417-8284b4f10fb5")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugType2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("id")]
    COR_TYPEID GetTypeID();

  }

}