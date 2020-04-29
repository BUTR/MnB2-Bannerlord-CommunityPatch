

namespace Antijank.Debugging {

  
  public enum CorDebugUnmappedStop {

    STOP_NONE,

    STOP_PROLOG,

    STOP_EPILOG,

    STOP_NO_MAPPING_INFO = 4,

    STOP_OTHER_UNMAPPED = 8,

    STOP_UNMANAGED = 16,

    STOP_ALL = 65535

  }

}