using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct COR_PRF_GC_GENERATION_RANGE {

    public COR_PRF_GC_GENERATION generation;

    public UIntPtr rangeStart;

    public UIntPtr rangeLength;

    public UIntPtr rangeLengthReserved;

  }

}