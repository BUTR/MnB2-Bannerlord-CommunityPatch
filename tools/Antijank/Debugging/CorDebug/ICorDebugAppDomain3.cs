﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("8CB96A16-B588-42E2-B71C-DD849FC2ECCC")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugAppDomain3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppTypesEnum")]
    ICorDebugTypeEnum GetCachedWinRTTypesForIIDs([In] uint cReqTypes,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid iidsToResolve);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppGuidToTypeEnum")]
    ICorDebugGuidToTypeEnum GetCachedWinRTTypes();

  }

}