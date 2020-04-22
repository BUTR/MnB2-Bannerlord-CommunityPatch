﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("6DC3FA01-D7CB-11D2-8A95-0080C792E5D8")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugEditAndContinueSnapshot {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CopyMetaData([MarshalAs(UnmanagedType.Interface)] [In]
      IStream pIStream, out Guid pMvid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMvid(out Guid pMvid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRoDataRVA(out uint pRoDataRVA);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRwDataRVA(out uint pRwDataRVA);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetPEBytes([MarshalAs(UnmanagedType.Interface)] [In]
      IStream pIStream);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILMap([In] uint mdFunction, [In] uint cMapSize, [In] ref COR_IL_MAP map);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetPESymbolBytes([MarshalAs(UnmanagedType.Interface)] [In]
      IStream pIStream);

  }

}