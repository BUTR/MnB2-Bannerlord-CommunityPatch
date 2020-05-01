using System;

namespace Antijank.Interop {

  [Flags]
  public enum PropertyStoreFlags {

    Default = 0,

    HandlerPropertiesOnly = 1 << 0,

    ReadWrite = 1 << 1,

    Temporary = 1 << 2,

    FastPropertiesOnly = 1 << 3,

    OpenSlowItem = 1 << 4,

    DelayCreation = 1 << 5,

    BestEffort = 1 << 6,

    NoOpLock = 1 << 7,

    PreferQueryProperties = 1 << 8,

    ExtrinsicProperties = 1 << 9,

    ExtrinsicPropertiesOnly = 1 << 10,

    VolatileProperties = 1 << 11,

    VolatilePropertiesOnly = 1 << 12,

    MaskValid = 0x1FFF

  }

}