using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public enum CorDebugUserState {

    USER_STOP_REQUESTED = 1,

    USER_SUSPEND_REQUESTED,

    USER_BACKGROUND = 4,

    USER_UNSTARTED = 8,

    USER_STOPPED = 16,

    USER_WAIT_SLEEP_JOIN = 32,

    USER_SUSPENDED = 64,

    USER_UNSAFE_POINT = 128,

    USER_THREADPOOL = 256

  }

}