using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComConversionLoss]
  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  [PublicAPI]
  public struct COR_PRF_ASSEMBLY_REFERENCE_INFO {

    public IntPtr pbPublicKeyOrToken;

    public uint cbPublicKeyOrToken;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string szName;

    [ComConversionLoss]
    public IntPtr pMetaData;

    public unsafe ref ASSEMBLYMETADATA MetaData(int i = 0)
      => ref Unsafe.AsRef<ASSEMBLYMETADATA>
        ((void*) (pMetaData + i * Unsafe.SizeOf<ASSEMBLYMETADATA>()));

    public IntPtr pbHashValue;

    public uint cbHashValue;

    public uint dwAssemblyRefFlags;

  }

}