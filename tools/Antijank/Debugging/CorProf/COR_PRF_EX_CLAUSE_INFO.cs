using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  [PublicAPI]
  public struct COR_PRF_EX_CLAUSE_INFO {

    public COR_PRF_CLAUSE_TYPE clauseType;

    public UIntPtr programCounter;

    public UIntPtr framePointer;

    public UIntPtr shadowStackPointer;

  }

}