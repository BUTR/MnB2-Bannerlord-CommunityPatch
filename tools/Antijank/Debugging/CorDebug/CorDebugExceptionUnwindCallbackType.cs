using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugExceptionUnwindCallbackType {

    DEBUG_EXCEPTION_UNWIND_BEGIN = 1,

    DEBUG_EXCEPTION_INTERCEPTED

  }

}