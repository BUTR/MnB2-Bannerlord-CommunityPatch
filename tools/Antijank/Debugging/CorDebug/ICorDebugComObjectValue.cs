﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("5F69C5E5-3E12-42DF-B371-F9D761D6EE24")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugComObjectValue {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCachedInterfaceTypes([In] int bIInspectableOnly,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugTypeEnum ppInterfacesEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCachedInterfacePointers([In] int bIInspectableOnly, [In] uint celt, out uint pceltFetched, out ulong ptrs);

  }

}