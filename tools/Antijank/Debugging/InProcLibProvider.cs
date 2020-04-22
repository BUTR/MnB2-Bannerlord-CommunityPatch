using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  public class InProcLibProvider : ICLRDebuggingLibraryProvider {

    public IntPtr FindLoadedLibrary(string name) {
      using (var proc = Process.GetCurrentProcess()) {
        foreach (ProcessModule module in proc.Modules) {
          if (module.FileName == name) {
            return module.BaseAddress;
          }

          var modFileName = Path.GetFileName(module.FileName);
          if (modFileName == name) {
            return module.BaseAddress;
          }

          var fileName = Path.GetFileName(name);
          if (modFileName == fileName)
            return module.BaseAddress;
        }
      }

      return default;
    }

    public void ProvideLibrary(string pWszFileName, uint dwTimestamp, uint dwSizeOfImage, out IntPtr phModule) {
      phModule = FindLoadedLibrary(pWszFileName);

      if (phModule != default)
        return;

      // module not already loaded, let's load it...

      var runtimeLibPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), pWszFileName);

      if (File.Exists(runtimeLibPath)) {
        phModule = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
          ? WindowsNatives.LoadLibrary(runtimeLibPath)
          : PosixNatives.LoadLibrary(runtimeLibPath);
      }

      if (phModule == default) {
        throw Marshal.GetExceptionForHR(
          unchecked((int) 0x80004005)); // E_FAIL
      }
    }

  }

}