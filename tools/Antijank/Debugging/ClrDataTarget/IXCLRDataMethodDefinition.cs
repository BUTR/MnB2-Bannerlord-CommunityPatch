using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("AAF60008-FB2C-420B-8FB1-42D244A54A97")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataMethodDefinition {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeDefinition([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition typeDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumInstances([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumInstance([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance instance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumInstances([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint flags, [In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTokenAndScope(out uint token, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataMethodDefinition method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLatestEnCVersion(out uint version);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumExtents(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumExtent([In] [Out] ref ulong handle, out CLRDATA_METHDEF_EXTENT extent);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumExtents([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeNotification(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetCodeNotification([In] uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRepresentativeEntryAddress(out ulong addr);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void HasClassOrMethodInstantiation(out int bGeneric);

  }

}