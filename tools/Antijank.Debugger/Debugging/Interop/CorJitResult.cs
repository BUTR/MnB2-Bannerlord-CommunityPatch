namespace Antijank.Interop {

  public enum CorJitResult : uint {

    Ok = 0,

    BadCode = 0x80000001u,

    OutOfMemory,

    InternalError,

    Skipped,

    RecoverableError

  }

}