﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("471C35B4-7C2F-4EF0-A945-00F8C38056F1")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICLRDataEnumMemoryRegions {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMemoryRegions([MarshalAs(UnmanagedType.Interface)] [In]
      ICLRDataEnumMemoryRegionsCallback callback, [In] uint miniDumpFlags, [In] CLRDataEnumMemoryFlags clrFlags);

  }

}