using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  [PublicAPI]
  public struct CLRDATA_MODULE_EXTENT {

    public ulong @base;

    public uint length;

    public CLRDataModuleExtentType type;

  }

}