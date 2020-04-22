﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("2C6269BD-2D13-4321-AE12-6686365FD6AF")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorProfilerObjectEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Clone([MarshalAs(UnmanagedType.Interface)] out ICorProfilerObjectEnum ppEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerObjectEnum objects, out uint pceltFetched);

  }

}