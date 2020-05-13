namespace Antijank.Interop {

  public enum CorInfoOptions {

    // zero initialize all variables
    InitLocals = 0x00000010,

    // is this shared generic code that access the generic context from the this pointer?  If so, then if the method has SEH then the 'this' pointer must always be reported and kept alive.
    GenericsContextFromThis = 0x00000020,

    // is this shared generic code that access the generic context from the ParamTypeArg(that is a MethodDesc)?  If so, then if the method has SEH then the 'ParamTypeArg' must always be reported and kept alive. Same as CORINFO_CALLCONV_PARAMTYPE
    GenericsContextFromMethodDesc = 0x00000040,

    // is this shared generic code that access the generic context from the ParamTypeArg(that is a MethodTable)?  If so, then if the method has SEH then the 'ParamTypeArg' must always be reported and kept alive. Same as CORINFO_CALLCONV_PARAMTYPE
    GenericsContextFromMethodTable = 0x00000080,

    GenericsContextMask = GenericsContextFromThis | GenericsContextFromMethodDesc | GenericsContextFromMethodTable,

    // Keep the generics context alive throughout the method even if there is no explicit use, and report its location to the CLR
    GenericsContextKeepAlive = 0x00000100,

  };

}