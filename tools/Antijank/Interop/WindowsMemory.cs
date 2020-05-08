using System;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Flags]
  public enum PageProtection {

    NoAccess = 1 << 0,

    ReadOnly = 1 << 1,

    ReadWrite = 1 << 2,

    WriteCopy = 1 << 3,

    Execute = 1 << 4,

    ExecuteRead = 1 << 5,

    ExecuteReadWrite = 1 << 6,

    ExecuteWriteCopy = 1 << 7,

    Guard = 1 << 8,

    NoCache = 1 << 9,

    WriteCombine = 1 << 10

  }

  public class WindowsMemory {

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool VirtualProtect(IntPtr address, uint regionSize, PageProtection newProtection, out PageProtection oldProtection);

    public static void WithUnprotectedRegion(IntPtr address, uint regionSize, Action action) {
      VirtualProtect(address, regionSize, PageProtection.ExecuteReadWrite, out var oldProtection);

      try {
        action();
      }
      finally {
        VirtualProtect(address, regionSize, oldProtection, out _);
      }
    }

  }

}