using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  [PublicAPI]
  public struct COR_FIELD_OFFSET {

    public int ridOfField;

    public uint ulOffset;

  }

}