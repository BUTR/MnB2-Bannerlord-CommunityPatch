﻿using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("879CAC0A-4A53-4668-B8E3-CB8473CB187F")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugRuntimeUnwindableFrame : ICorDebugFrame {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionToken(out uint pToken);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStackRange(out ulong pStart, out ulong pEnd);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);
    */

  }

}