﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCB0B-8A68-11D2-983C-0000F808342D")]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugRegisterSet {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRegistersAvailable(out ulong pAvailable);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRegisters([In] ulong mask, [In] uint regCount, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugRegisterSet regBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetRegisters([In] ulong mask, [In] uint regCount, [In] ref ulong regBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadContext([In] uint contextSize, [MarshalAs(UnmanagedType.Interface)] [In] [Out]
      ICorDebugRegisterSet context);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetThreadContext([In] uint contextSize, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugRegisterSet context);

  }

}