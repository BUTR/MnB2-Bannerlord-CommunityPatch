using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct CLRDATA_ADDRESS_RANGE {

    public ulong startAddress;

    public ulong endAddress;

  }

}