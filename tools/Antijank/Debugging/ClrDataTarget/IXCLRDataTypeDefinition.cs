using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("4675666C-C275-45B8-9F6C-AB165D5C1E09")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface IXCLRDataTypeDefinition {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModule([MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodDefinitions(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodDefinition([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition methodDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodDefinitions([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodDefinitionsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodDefinitionByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodDefinitionsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodDefinitionByToken([In] uint token,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition methodDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumInstances([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumInstance([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance instance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumInstances([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint flags, [In] uint bufLen, out uint nameLen, out ushort nameBuf);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTokenAndScope(out uint token, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCorElementType(out uint type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeDefinition type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void Request([In] uint reqCode, [In] uint inBufferSize, [In] ref byte* inBuffer, [In] uint outBufferSize,
      ref byte* outBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArrayRank(out uint rank);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetBase([MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition @base);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNumFields([In] uint flags, out uint numFields);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumFields([In] uint flags, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumField([In] [Out] ref ulong handle, [In] uint nameBufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumFields([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumFieldsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint nameFlags,
      [In] uint fieldFlags, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumFieldByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumFieldsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldByToken([In] uint token, [In] uint nameBufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeNotification(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTypeNotification([In] uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumField2([In] [Out] ref ulong handle, [In] uint nameBufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumFieldByName2([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataModule tokenScope, out uint token);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldByToken2([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule tokenScope, [In] uint token, [In] uint nameBufLen, out uint nameLen, out ushort nameBuf,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type, out uint flags);

  }

}