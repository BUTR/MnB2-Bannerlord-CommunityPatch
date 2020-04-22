﻿using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum CLRDataEnumMemoryFlags {

    CLRDATA_ENUM_MEM_DEFAULT,

    CLRDATA_ENUM_MEM_MINI = 0,

    CLRDATA_ENUM_MEM_HEAP,

    CLRDATA_ENUM_MEM_TRIAGE

  }

}