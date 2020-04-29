using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct COR_PRF_FUNCTION_ARGUMENT_INFO {

    public uint numRanges;

    public uint totalArgumentSize;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public COR_PRF_FUNCTION_ARGUMENT_RANGE[] ranges;

  }

}