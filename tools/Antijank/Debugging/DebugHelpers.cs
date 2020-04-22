using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  public static class DebugHelpers {

    public static IntPtr GetRuntimeModuleInfo(out string filePath) {
      using (var proc = Process.GetCurrentProcess())
        return GetRuntimeModuleInfo(proc, out filePath);
    }

    public static IntPtr GetRuntimeModuleInfo(Process proc, out string filePath) {
      {
        var rtDir = RuntimeEnvironment.GetRuntimeDirectory();

        foreach (ProcessModule module in proc.Modules) {
          if (!module.FileName.StartsWith(rtDir))
            continue;

          if (!module.FileVersionInfo.FileDescription.Contains(".NET Runtime"))
            continue;

          filePath = module.FileName;
          return module.BaseAddress;
        }
      }

      throw new DllNotFoundException("Couldn't find .NET Runtime module.");
    }

  }

}