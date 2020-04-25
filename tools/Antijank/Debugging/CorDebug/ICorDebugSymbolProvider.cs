using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("3948A999-FD8A-4C38-A708-8A71E9B04DBB")]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugSymbolProvider {

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetStaticFieldSymbols([In] uint cbSignature, [In] ref byte* typeSig, [In] uint cRequestedSymbols,
      out uint pcFetchedSymbols, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider pSymbols);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetInstanceFieldSymbols([In] uint cbSignature, [In] ref byte* typeSig, [In] uint cRequestedSymbols,
      out uint pcFetchedSymbols, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider pSymbols);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodLocalSymbols([In] uint nativeRVA, [In] uint cRequestedSymbols, out uint pcFetchedSymbols,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider pSymbols);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodParameterSymbols([In] uint nativeRVA, [In] uint cRequestedSymbols, out uint pcFetchedSymbols,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider pSymbols);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMergedAssemblyRecords([In] uint cRequestedRecords, out uint pcFetchedRecords,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider pRecords);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetMethodProps([In] uint codeRva, out uint pMethodToken, out uint pcGenericParams, [In] uint cbSignature,
      out uint pcbSignature, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider signature);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTypeProps([In] uint vtableRva, [In] uint cbSignature, out uint pcbSignature,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugSymbolProvider signature);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeRange([In] uint codeRva, out uint pCodeStartAddress, ref uint pCodeSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssemblyImageBytes([In] ulong rva, [In] uint length,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugMemoryBuffer ppMemoryBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void GetObjectSize([In] uint cbSignature, [In] ref byte* typeSig, out uint pObjectSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssemblyImageMetadata([MarshalAs(UnmanagedType.Interface)] out ICorDebugMemoryBuffer ppMemoryBuffer);

  }

}