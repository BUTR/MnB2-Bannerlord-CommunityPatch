using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  [PublicAPI]
  public struct COR_PRF_GC_GENERATION_RANGE {

    public COR_PRF_GC_GENERATION generation;

    public UIntPtr rangeStart;

    public UIntPtr rangeLength;

    public UIntPtr rangeLengthReserved;

  }

}