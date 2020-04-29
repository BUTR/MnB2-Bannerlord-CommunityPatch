using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  
  public struct GcEvtArgs {

    public GcEvt_t typ;

    public int condemnedGeneration;

  }

}