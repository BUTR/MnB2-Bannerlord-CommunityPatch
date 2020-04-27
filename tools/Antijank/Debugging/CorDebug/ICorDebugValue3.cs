using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("565005FC-0F8A-4F3E-9EDB-83102B156595")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugValue3 {

    [return: Description("pSize")]
    ulong GetSize64();

  }

}