using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("2FA17588-43C2-46AB-9B51-C8F01E39C9AC")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataAssembly {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumModules(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumModule([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumModules([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFileName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAssembly assembly);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumAppDomains(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumAppDomain([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataAppDomain appDomain);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumAppDomains([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDisplayName([In] uint bufLen, out uint nameLen, out ushort name);

  }

}