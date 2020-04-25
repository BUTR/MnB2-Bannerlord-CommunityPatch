using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E5F3039D-2C0C-4230-A69E-12AF1C3E563C")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRLibrarySupport {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void LoadHardboundDependency(ref ushort name, [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid mvid, out UIntPtr loadedBase);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void LoadSoftboundDependency(ref ushort name, ref byte* assemblymetadataBinding, ref byte* hash,
      uint hashLength, out UIntPtr loadedBase);

  }

}