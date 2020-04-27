using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  
  public struct COR_DEBUG_STEP_RANGE {

    public uint startOffset;

    public uint endOffset;

  }

}