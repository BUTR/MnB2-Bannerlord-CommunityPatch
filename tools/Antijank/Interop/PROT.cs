using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Antijank.Interop {

  
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [Flags]
  public enum PROT : int {
    READ       = 0x1,  // Page can be read.
    WRITE      = 0x2,  // Page can be written.
    EXEC       = 0x4,  // Page can be executed.
    NONE       = 0x0,  // Page can not be accessed.
    GROWSDOWN  = 0x01000000, // Extend change to start of growsdown vma (mprotect only).
    GROWSUP    = 0x02000000, // Extend change to start of growsup vma (mprotect only).
  }

}