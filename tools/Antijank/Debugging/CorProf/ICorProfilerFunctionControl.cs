﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("F0963021-E1EA-4732-8581-E01B0BD3C0C6")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorProfilerFunctionControl {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetCodegenFlags([In] uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILFunctionBody([In] uint cbNewILMethodHeader, [In] ref byte pbNewILMethodHeader);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILInstrumentedCodeMap([In] uint cILMapEntries, [In] ref COR_IL_MAP rgILMapEntries);

  }

}