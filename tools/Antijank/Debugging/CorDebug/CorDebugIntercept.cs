using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugIntercept {

    INTERCEPT_NONE,

    INTERCEPT_CLASS_INIT,

    INTERCEPT_EXCEPTION_FILTER,

    INTERCEPT_SECURITY = 4,

    INTERCEPT_CONTEXT_POLICY = 8,

    INTERCEPT_INTERCEPTION = 16,

    INTERCEPT_ALL = 65535

  }

}