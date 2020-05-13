using System;

namespace Antijank.Interop {

  public struct CORINFO_METHOD_INFO {

    public IntPtr ftn;

    public IntPtr scope;

    public IntPtr ILCode;

    public int ILCodeSize;

    public uint maxStack;

    public uint EHcount;

    public CorInfoOptions options;

    public CorInfoRegionKind regionKind;

    public CORINFO_SIG_INFO args;

    public CORINFO_SIG_INFO locals;

  };

}