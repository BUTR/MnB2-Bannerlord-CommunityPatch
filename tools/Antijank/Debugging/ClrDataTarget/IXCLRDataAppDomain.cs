﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("7CA04601-C702-4670-A63C-FA44F7DA7BD5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataAppDomain {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetProcess([MarshalAs(UnmanagedType.Interface)] out IXCLRDataProcess process);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetUniqueID(out ulong id);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetManagedObject([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

  }

}