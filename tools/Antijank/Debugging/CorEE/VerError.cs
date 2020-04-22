using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComConversionLoss]
  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  [PublicAPI]
  public struct VerError {

    public uint flags;

    public uint opcode;

    public uint uOffset;

    public uint Token;

    public uint item1_flags;

    [ComConversionLoss]
    public IntPtr item1_data;

    public uint item2_flags;

    [ComConversionLoss]
    public IntPtr item2_data;

  }

}