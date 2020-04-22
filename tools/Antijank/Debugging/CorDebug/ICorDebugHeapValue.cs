﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("CC7BCAFA-8A68-11D2-983C-0000F808342D")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugHeapValue : ICorDebugValue {

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
    void IsValid(out int pbValid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppBreakpoint")]
    ICorDebugValueBreakpoint CreateRelocBreakpoint();

  }

}