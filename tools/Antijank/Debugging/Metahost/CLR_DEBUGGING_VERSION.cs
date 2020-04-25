using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 2)]
  [PublicAPI]
  public struct CLR_DEBUGGING_VERSION {

    public ushort wStructVersion;

    public ushort wMajor;

    public ushort wMinor;

    public ushort wBuild;

    public ushort wRevision;

  }

}