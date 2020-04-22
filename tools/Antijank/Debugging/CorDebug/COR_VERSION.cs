﻿using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  [PublicAPI]
  public struct COR_VERSION {

    public uint dwMajor;

    public uint dwMinor;

    public uint dwBuild;

    public uint dwSubBuild;

  }

}