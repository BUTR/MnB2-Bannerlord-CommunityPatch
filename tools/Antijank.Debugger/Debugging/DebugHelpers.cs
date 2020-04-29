using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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

    public delegate int ComEnumerableNextDelegate1<T>(uint celt, T[] values, out uint fetched);

    public delegate void ComEnumerableNextDelegate2<T>(uint celt, T[] values, out uint fetched);

    public static IEnumerable<T> GetEnumerable<T>(ComEnumerableNextDelegate1<T> next, Func<int, bool> hrCheck = null) {
      for (;;) {
        var buf = new T[1];
        var hr = next(1, buf, out var fetched);
        if (hr != 0 && (!hrCheck?.Invoke(hr) ?? false))
          yield break;

        if (fetched < 1)
          yield break;

        yield return buf[0];
      }
    }

    public static IEnumerable<T> GetEnumerable<T>(ComEnumerableNextDelegate2<T> next) {
      for (;;) {
        var buf = new T[1];
        next(1, buf, out var fetched);

        if (fetched < 1)
          yield break;

        yield return buf[0];
      }
    }

    public delegate int ComGetStringDelegate1(uint celt, out uint len, StringBuilder buf);

    public delegate void ComGetStringDelegate2(uint celt, out uint len, StringBuilder buf);

    public static string GetString(ComGetStringDelegate1 getStr, int bufSize = 4096, Func<int, bool> hrCheck = null) {
      var sb = new StringBuilder(bufSize);
      var hr = getStr((uint) bufSize, out var len, sb);
      if (hr != 0 && (!hrCheck?.Invoke(hr) ?? false))
        throw Marshal.GetExceptionForHR(hr);

      return sb.ToString(0, (int) len - 1);
    }

    public static string GetString(ComGetStringDelegate2 getStr, int bufSize = 4096, Func<int, bool> hrCheck = null) {
      var sb = new StringBuilder(bufSize);

      getStr((uint) bufSize, out var len, sb);

      return sb.ToString(0, (int) len - 1);
    }

  }

}