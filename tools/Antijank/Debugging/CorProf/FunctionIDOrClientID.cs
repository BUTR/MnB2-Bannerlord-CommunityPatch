using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 8)]
  [PublicAPI]
  public struct FunctionIDOrClientID {

    [FieldOffset(0)]
    public UIntPtr functionId;

    [FieldOffset(0)]
    public UIntPtr clientID;

  }

}