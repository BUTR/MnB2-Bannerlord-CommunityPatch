﻿using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  [PublicAPI]
  public struct GcEvtArgs {

    public GcEvt_t typ;

    public int condemnedGeneration;

  }

}