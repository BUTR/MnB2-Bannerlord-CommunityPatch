﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCAF5-8A68-11D2-983C-0000F808342D")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugClass {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pModule")]
    ICorDebugModule GetModule();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pTypeDef")]
    uint GetToken();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppValue")]
    ICorDebugValue GetStaticFieldValue([In] uint fieldDef, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFrame pFrame);

  }

}