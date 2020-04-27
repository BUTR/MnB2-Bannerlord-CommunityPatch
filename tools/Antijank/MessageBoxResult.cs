using JetBrains.Annotations;

namespace Antijank {

  
  public enum MessageBoxResult : int {

    Error = 0,

    Ok = 1,

    Cancel,

    Abort,

    Retry,

    Ignore,

    Yes,

    No,

    TryAgain = 10,

    Continue

  }

}