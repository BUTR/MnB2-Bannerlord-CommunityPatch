using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("F8544EC3-5E4E-46C7-8D3E-A52B8405B1F5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugThread3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppStackWalk")]
    ICorDebugStackWalk CreateStackWalk();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetActiveInternalFrames([In] uint cInternalFrames, out uint pcInternalFrames,
      [MarshalAs(UnmanagedType.Interface)] [In] [Out]
      ICorDebugThread3 ppInternalFrames);

  }

}