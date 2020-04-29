using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Antijank.Debugging {

  
  [Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93"), ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataAssemblyImport {

    unsafe uint GetAssemblyProps(uint mda, out byte* ppbPublicKey, out uint pcbPublicKey, out uint pulHashAlgId,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      StringBuilder szName, uint cchName, out uint pchName, ref ASSEMBLYMETADATA pMetaData, out uint pdwAssemblyFlags);

    unsafe uint GetAssemblyRefProps(uint mdar, out byte* ppbPublicKeyOrToken, out uint pcbPublicKeyOrToken,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      char[] szName, uint cchName, out uint pchName, ref ASSEMBLYMETADATA pMetaData, out IntPtr ppbHashValue, out uint pcbHashValue, out uint pdwAssemblyRefFlags);

    int GetFileProps(uint mdf, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      char[] szName, uint cchName, out uint pchName, out IntPtr ppbHashValue, out uint pcbHashValue, out uint pdwFileFlags);

    int GetExportedTypeProps(uint mdct, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      char[] szName, uint cchName, out uint pchName, out uint ptkImplementation, out uint ptkTypeDef, out uint pdwExportedTypeFlags);

    int GetManifestResourceProps(uint mdmr, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      char[] szName, uint cchName, out uint pchName, out uint ptkImplementation, out uint pdwOffset, out uint pdwResourceFlags);

    uint EnumAssemblyRefs(ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      uint[] rAssemblyRefs, uint cMax, out uint pcTokens);

    int EnumFiles(ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      uint[] rFiles, uint cMax, out uint pcTokens);

    uint EnumExportedTypes(ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      uint[] rExportedTypes, uint cMax, out uint pcTokens);

    int EnumManifestResources(ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      uint[] rManifestResources, uint cMax, out uint pcTokens);

    uint GetAssemblyFromScope(out uint ptkAssembly);

    int FindExportedTypeByName([MarshalAs(UnmanagedType.LPWStr)] string szName, uint mdtExportedType, out uint ptkExportedType);

    int FindManifestResourceByName([MarshalAs(UnmanagedType.LPWStr)] string szName, out uint ptkManifestResource);

    void CloseEnum(IntPtr hEnum);

    int FindAssembliesByName([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      object[] ppIUnk, uint cMax, out uint pcAssemblies);

  }

}