using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CorDebugMappingResult {

    MAPPING_PROLOG = 1,

    MAPPING_EPILOG,

    MAPPING_NO_INFO = 4,

    MAPPING_UNMAPPED_ADDRESS = 8,

    MAPPING_EXACT = 16,

    MAPPING_APPROXIMATE = 32

  }

}