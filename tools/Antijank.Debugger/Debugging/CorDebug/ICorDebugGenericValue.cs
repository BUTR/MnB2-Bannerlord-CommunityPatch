using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("CC7BCAF8-8A68-11D2-983C-0000F808342D")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugGenericValue : ICorDebugValue {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetType(out uint pType);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out uint pSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong pAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pTo")]
    IntPtr GetValue();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetValue([In] IntPtr pFrom);

  }

}