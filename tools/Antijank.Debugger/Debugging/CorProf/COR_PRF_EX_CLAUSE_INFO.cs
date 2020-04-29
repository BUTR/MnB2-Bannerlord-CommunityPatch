using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct COR_PRF_EX_CLAUSE_INFO {

    public COR_PRF_CLAUSE_TYPE clauseType;

    public UIntPtr programCounter;

    public UIntPtr framePointer;

    public UIntPtr shadowStackPointer;

  }

}