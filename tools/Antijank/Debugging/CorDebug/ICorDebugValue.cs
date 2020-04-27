using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("CC7BCAF7-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugValue {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pType")]
    CorElementType GetType();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pSize")]
    uint GetSize();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong pAddress);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppBreakpoint")]
    ICorDebugValueBreakpoint CreateBreakpoint();

  }

}