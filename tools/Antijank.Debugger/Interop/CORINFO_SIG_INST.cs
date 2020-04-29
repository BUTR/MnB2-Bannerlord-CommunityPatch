using System;

namespace Antijank.Interop {

  public struct CORINFO_SIG_INST
  {
    public uint classInstCount;
    public IntPtr classInst; // (representative, not exact) instantiation for class type variables in signature
    public uint methInstCount;
    public IntPtr methInst; // (representative, not exact) instantiation for method type variables in signature
  };

}