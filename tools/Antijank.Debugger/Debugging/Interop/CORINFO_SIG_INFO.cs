using System;

namespace Antijank.Interop {

  public struct CORINFO_SIG_INFO {

    public CorInfoCallConv callConv;

    public IntPtr retTypeClass; // if the return type is a value class, this is its handle (enums are normalized)

    public IntPtr retTypeSigClass; // returns the value class as it is in the sig (enums are not converted to primitives)

    public CorInfoType retType;

    public byte flags;

    public ushort numArgs;

    public CORINFO_SIG_INST sigInst; // information about how type variables are being instantiated in generic code

    public IntPtr args;

    public IntPtr pSig;

    public uint cbSig;

    public IntPtr scope; // passed to getArgClass

    public uint token;

    public long garbage;

  };

}