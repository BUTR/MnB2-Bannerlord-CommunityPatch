using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct CLRDATA_MODULE_EXTENT {

    public ulong @base;

    public uint length;

    public CLRDataModuleExtentType type;

  }

}