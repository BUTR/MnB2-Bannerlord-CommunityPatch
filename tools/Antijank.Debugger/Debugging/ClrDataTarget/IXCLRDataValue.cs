using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("96EC93C7-1000-4E93-8991-98D8766E6666")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataValue {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong address);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out ulong size);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetBytes([In] uint bufLen, out uint dataSize, ref byte* buffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void SetBytes([In] uint bufLen, out uint dataSize, [In] ref byte* buffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetType([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeInstance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumFields(out uint numFields);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldByIndex([In] uint index, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [In] uint bufLen, out uint nameLen, out ushort nameBuf, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumFields2([In] uint flags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance fromType, out uint numFields);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumFields([In] uint flags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance fromType, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumField([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [In] uint nameBufLen, out uint nameLen, out ushort nameBuf, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumFields([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumFieldsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint nameFlags,
      [In] uint fieldFlags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance fromType, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumFieldByName([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumFieldsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldByToken([In] uint token, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssociatedValue([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue assocValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssociatedType([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance assocType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetString([In] uint bufLen, out uint strLen, out ushort str);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArrayProperties(out uint rank, out uint totalElements, [In] uint numDim, out uint dims, [In] uint numBases,
      out int bases);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArrayElement([In] uint numInd, [In] ref int indices,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumField2([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [In] uint nameBufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumFieldByName2([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldByToken2([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule tokenScope, [In] uint token, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field,
      [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumLocations(out uint numLocs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetLocationByIndex([In] uint loc, out uint flags, out ulong arg);

  }

}