using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public enum CorDebugBlockingReason {

    BLOCKING_NONE = 0x0,

    BLOCKING_MONITOR_CRITICAL_SECTION = 0x1,

    BLOCKING_MONITOR_EVENT = 0x2

  }

}