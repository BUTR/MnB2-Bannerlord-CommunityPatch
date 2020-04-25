using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("88E32849-0A0A-4CB0-9022-7CD2E9E139E2")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDataModule {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumAssemblies(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumAssembly([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataAssembly assembly);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumAssemblies([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumTypeDefinitions(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumTypeDefinition([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition typeDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumTypeDefinitions([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumTypeInstances([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumTypeInstance([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance typeInstance);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumTypeInstances([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumTypeDefinitionsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumTypeDefinitionByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumTypeDefinitionsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumTypeInstancesByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumTypeInstanceByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeInstance type);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumTypeInstancesByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeDefinitionByToken([In] uint token,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataTypeDefinition typeDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodDefinitionsByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodDefinitionByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodDefinitionsByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumMethodInstancesByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMethodInstanceByName([In] [Out] ref ulong handle,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumMethodInstancesByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodDefinitionByToken([In] uint token,
      [MarshalAs(UnmanagedType.Interface)] out IXCLRDataMethodDefinition methodDefinition);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumDataByName([MarshalAs(UnmanagedType.LPWStr)] [In] string name, [In] uint flags,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataAppDomain appDomain, [MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask tlsTask, out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumDataByName([In] [Out] ref ulong handle, [MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumDataByName([In] ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFileName([In] uint bufLen, out uint nameLen, out ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFlags(out uint flags);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsSameObject([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void StartEnumExtents(out ulong handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumExtent([In] [Out] ref ulong handle, out CLRDATA_MODULE_EXTENT extent);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndEnumExtents([In] ulong handle);

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
    void GetVersionId(out Guid vid);

  }

}