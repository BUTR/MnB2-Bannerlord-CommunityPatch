using System;
using System.Runtime.InteropServices;
using Antijank.Interop;

namespace Antijank.Debugging {

  [ComConversionLoss]
  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct ASSEMBLYMETADATA {

    public ushort usMajorVersion;

    public ushort usMinorVersion;

    public ushort usBuildNumber;

    public ushort usRevisionNumber;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string szLocale;

    public uint cbLocale;

    
    public unsafe uint* rProcessor;

    public uint ulProcessor;

    public unsafe OSINFO* rOS;

    public uint ulOS;

  }

}