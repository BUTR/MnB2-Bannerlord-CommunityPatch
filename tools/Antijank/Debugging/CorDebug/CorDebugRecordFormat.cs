using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugRecordFormat {

    None = 0,

    FORMAT_WINDOWS_EXCEPTIONRECORD32 = 1,

    FORMAT_WINDOWS_EXCEPTIONRECORD64 = 2

  }

}