using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public enum CorDebugCodeInvokePurpose {

    CODE_INVOKE_PURPOSE_NONE,

    CODE_INVOKE_PURPOSE_NATIVE_TO_MANAGED_TRANSITION, // The managed code will run any managed entrypoint

    // such as a reverse p-invoke. Any more detailed purpose
    // is unknown by the runtime.
    CODE_INVOKE_PURPOSE_CLASS_INIT, // The managed code will run a static constructor

    CODE_INVOKE_PURPOSE_INTERFACE_DISPATCH // The managed code will run the implementation for
    // some interface method that was called

  }

}