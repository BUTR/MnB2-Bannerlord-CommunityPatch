using System;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct PropertyKey {

    public Guid fmtid;

    public uint pid;

  }

}