using System;


namespace Antijank.Interop {

  
  [Flags]
  public enum StorageMode : int {

    // Access
    Read = 0,

    Write = 1 << 0,

    ReadWrite = 1 << 1,

    // Sharing

    ShareExclusive = 1 << 4,

    ShareDenyWrite = 1 << 5,

    ShareDenyRead = ShareExclusive | ShareDenyWrite,

    ShareDenyNone = 1 << 6,

    Priority = 1 << 18,

    // Creation

    FailIfThere = 0,

    Create = 1 << 12,

    Convert = 1 << 17,

    // Transactioning

    Direct = 0,

    Transacted = 1 << 16,

    // Transactioning Performance

    NoScratch = 1 << 20,

    NoSnapshot = 1 << 21,

    // Other

    DirectSingleWriterMultipleReader = 1 << 22,

    Simple = 1 << 27,

    // Delete On Release

    DeleteOnRelease = 1 << 26,

  }

}