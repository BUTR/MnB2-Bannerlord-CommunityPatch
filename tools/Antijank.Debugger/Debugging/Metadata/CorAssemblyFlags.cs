namespace Antijank.Debugging {

  public enum CorAssemblyFlags {

    PublicKey = 0x0001,

    MSIL = 0x0010,

    x86 = 0x0020,

    IA64 = 0x0030,

    AMD64 = 0x0040,

    ARM = 0x0050,

    ArchNoPlatform = 0x0070,

    ArchSpecified = 0x0080,

    ArchMask = 0x0070,

    ArchFullMask = 0x00F0,

    ArchShift = 0x0004,

    EnableJITcompileTracking = 0x8000,

    DisableJITcompileOptimizer = 0x4000,

    Retargetable = 0x0100,

    WindowsRuntime = 0x0200,

    ContentTypeMask = 0x0E00,

  }

}