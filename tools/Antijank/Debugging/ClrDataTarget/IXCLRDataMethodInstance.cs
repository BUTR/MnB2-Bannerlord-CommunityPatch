using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("ECD73800-22CA-4B0D-AB55-E9BA7E6318A5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataMethodInstance {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeInstance([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeInstance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDefinition([MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition methodDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTokenAndScope(out uint token, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint flags, [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetEnCVersion(out uint version);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumTypeArguments(out uint numTypeArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeArgumentByIndex([In] uint index,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeArg);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILOffsetsByAddress([In] ulong address, [In] uint offsetsLen, out uint offsetsNeeded, out uint ilOffsets);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddressRangesByILOffset([In] uint ilOffset, [In] uint rangesLen, out uint rangesNeeded,
      out CLRDATA_ADDRESS_RANGE addressRanges);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILAddressMap([In] uint mapLen, out uint mapNeeded, out CLRDATA_IL_ADDRESS_MAP maps);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumExtents(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumExtent([In] [Out] ref ulong handle, out CLRDATA_ADDRESS_RANGE extent);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumExtents([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRepresentativeEntryAddress(out ulong addr);

  }

}