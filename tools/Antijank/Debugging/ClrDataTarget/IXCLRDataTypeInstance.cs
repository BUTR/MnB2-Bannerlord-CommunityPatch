using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("4D078D91-9CB3-4B0D-97AC-28C8A5A82597")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataTypeInstance {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodInstances(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodInstance([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance methodInstance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodInstances([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodInstancesByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodInstanceByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodInstancesByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumStaticFields(out uint numFields);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStaticFieldByIndex([In] uint index, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field, [In] uint bufLen,
      out uint nameLen, out ushort nameBuf, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumStaticFieldsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumStaticFieldByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumStaticFieldsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumTypeArguments(out uint numTypeArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeArgumentByIndex([In] uint index,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeArg);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint flags, [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModule([MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDefinition([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition typeDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumStaticFields2([In] uint flags, out uint numFields);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumStaticFields([In] uint flags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumStaticField([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumStaticFields([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumStaticFieldsByName2([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint nameFlags,
      [In] uint fieldFlags, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumStaticFieldByName2([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumStaticFieldsByName2([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStaticFieldByToken([In] uint token, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field, [In] uint bufLen,
      out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetBase([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance @base);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumStaticField2([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value,
      [In] uint bufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumStaticFieldByName3([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStaticFieldByToken2([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule tokenScope, [In] uint token, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue field, [In] uint bufLen,
      out uint nameLen, out ushort nameBuf);

  }

}