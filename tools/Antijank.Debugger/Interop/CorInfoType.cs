namespace Antijank.Interop {

  public enum CorInfoType : byte {

    Undefined,

    Void,

    Bool,

    Char,

    SByte,

    Byte,

    Short,

    UShort,

    Int,

    UInt,

    Long,

    ULong,

    IntPtr,

    UIntPtr,

    Float,

    Double,

    Pointer = 0x11,

    ByRef,

    ValueType,

    Class,

    RefAny,

    GenericVariable

  };

}