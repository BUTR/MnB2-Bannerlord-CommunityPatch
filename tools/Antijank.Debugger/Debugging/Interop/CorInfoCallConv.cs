namespace Antijank.Interop {

  public enum CorInfoCallConv {

    // These correspond to CorCallingConvention

    Default,

    C,

    StdCall,

    ThisCall,

    FastCall,

    VarArg,

    Field,

    LocalSig,

    Property,

    NativeVarArg = 0xb,

    Mask = 0x0f,

    Generic = 0x10,

    HasThis = 0x20,

    ExplicitThis = 0x40,

    ParamType = 0x80

  };

}