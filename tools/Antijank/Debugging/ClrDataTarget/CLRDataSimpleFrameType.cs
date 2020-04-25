using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CLRDataSimpleFrameType {

    CLRDATA_SIMPFRAME_UNRECOGNIZED = 1,

    CLRDATA_SIMPFRAME_MANAGED_METHOD,

    CLRDATA_SIMPFRAME_RUNTIME_MANAGED_CODE = 4,

    CLRDATA_SIMPFRAME_RUNTIME_UNMANAGED_CODE = 8

  }

}