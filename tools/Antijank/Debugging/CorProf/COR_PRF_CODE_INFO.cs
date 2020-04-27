using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct COR_PRF_CODE_INFO {

    public UIntPtr startAddress;

    public UIntPtr size;

  }

}