/*
// client
IHostFilter 
IMapToken
IMetaDataError 
// host
IMetaDataConverter
IMetaDataInfo 
*/

using System;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  [ComImport, Guid("211EF15B-5317-4438-B196-DEC87B887693"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataAssemblyEmit {

    int DefineAssembly([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pbPublicKey, uint cbPublicKey, uint ulHashAlgId, [MarshalAs(UnmanagedType.LPWStr)] string szName, IntPtr pMetaData, int dwAssemblyFlags, out int pma);

    int DefineAssemblyRef([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      byte[] pbPublicKeyOrToken, uint cbPublicKeyOrToken, [MarshalAs(UnmanagedType.LPWStr)] string szName, IntPtr pMetaData,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      byte[] pbHashValue, uint cbHashValue, uint dwAssemblyRefFlags, out uint assemblyRefToken);

    int DefineFile([MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, uint dwFileFlags, out uint fileToken);

    int DefineExportedType([MarshalAs(UnmanagedType.LPWStr)] string szName, uint tkImplementation, uint tkTypeDef, uint dwExportedTypeFlags, out uint pmdct);

    int DefineManifestResource([MarshalAs(UnmanagedType.LPWStr)] string szName, uint tkImplementation, uint dwOffset, uint dwResourceFlags, out uint pmdmr);

    int SetAssemblyProps(uint pma, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbPublicKey, uint cbPublicKey, uint ulHashAlgId, [MarshalAs(UnmanagedType.LPWStr)] string szName, IntPtr pMetaData, uint dwAssemblyFlags);

    int SetAssemblyRefProps(uint ar, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbPublicKeyOrToken, uint cbPublicKeyOrToken, [MarshalAs(UnmanagedType.LPWStr)] string szName, IntPtr pMetaData,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      byte[] pbHashValue, uint cbHashValue, uint dwAssemblyRefFlags);

    int SetFileProps(uint file, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      byte[] pbHashValue, uint cbHashValue, uint dwFileFlags);

    int SetExportedTypeProps(uint ct, uint tkImplementation, uint tkTypeDef, uint dwExportedTypeFlags);

    int SetManifestResourceProps(uint mr, uint tkImplementation, uint dwOffset, uint dwResourceFlags);

  };

}