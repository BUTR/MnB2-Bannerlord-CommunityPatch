using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugSetContextFlag {

    SET_CONTEXT_FLAG_ACTIVE_FRAME = 1,

    SET_CONTEXT_FLAG_UNWIND_FRAME

  }

}