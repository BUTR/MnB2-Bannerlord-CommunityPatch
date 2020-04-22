using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  public class InProcLibProvider : ICLRDebuggingLibraryProvider {

    public void ProvideLibrary(string pWszFileName, uint dwTimestamp, uint dwSizeOfImage, out IntPtr phModule) {
      phModule = default;

      using (var proc = Process.GetCurrentProcess()) {
        foreach (ProcessModule module in proc.Modules) {
          if (module.FileName == pWszFileName) {
            phModule = module.BaseAddress;
            return;
          }

          var modFileName = Path.GetFileName(module.FileName);
          if (modFileName == pWszFileName) {
            phModule = module.BaseAddress;
            return;
          }

          var fileName = Path.GetFileName(pWszFileName);
          if (modFileName != fileName)
            continue;

          phModule = module.BaseAddress;
          return;
        }
      }

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