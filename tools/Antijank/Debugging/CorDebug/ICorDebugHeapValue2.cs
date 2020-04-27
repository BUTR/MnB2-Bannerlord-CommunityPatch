using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E3AC4D6C-9CB7-43E6-96CC-B21540E5083C")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugHeapValue2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppHandle")]
    ICorDebugHandleValue CreateHandle([In] CorDebugHandleType type);

  }

}