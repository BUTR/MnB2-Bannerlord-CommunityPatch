

namespace Antijank.Debugging {

  
  public enum CorDebugCodeInvokeKind {

    CODE_INVOKE_KIND_NONE, // if there is any managed code invoked by this method, it will have

    // have to be located by explicit events/breakpoints later
    // OR we may just miss some of the managed code this method calls
    // because there is no easy way to stop on it
    // OR the method may never invoke managed code
    CODE_INVOKE_KIND_RETURN, // This method will invoke managed code via a return instruction,

    // Stepping out should arrive at the next managed code
    CODE_INVOKE_KIND_TAILCALL // This method will invoke managed code via a tail-call. Single-stepping
    // + stepping over any call instructions should arrive at managed code

  }

}