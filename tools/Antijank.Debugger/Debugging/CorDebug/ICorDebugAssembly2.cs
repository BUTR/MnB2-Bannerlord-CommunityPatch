using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("426D1F9E-6DD4-44C8-AEC7-26CDBAF4E398")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugAssembly2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [return: Description("pbFullyTrusted")]
    bool IsFullyTrusted();

  }

}