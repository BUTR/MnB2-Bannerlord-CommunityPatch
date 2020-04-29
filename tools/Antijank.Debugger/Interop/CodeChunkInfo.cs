using System.Runtime.InteropServices;

namespace Antijank.Interop {

  // Token: 0x02000075 RID: 117
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct CodeChunkInfo {

    // Token: 0x04000149 RID: 329
    public ulong startAddr;

    // Token: 0x0400014A RID: 330
    public uint length;

  }

}