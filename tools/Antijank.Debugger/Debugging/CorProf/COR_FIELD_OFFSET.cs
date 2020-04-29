using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  
  public struct COR_FIELD_OFFSET {

    public int ridOfField;

    public uint ulOffset;

  }

}