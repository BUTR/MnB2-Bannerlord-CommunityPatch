using System;
using System.Runtime.InteropServices;
using Antijank.Interop;

namespace Antijank {

  internal partial class PosixNatives {

    //const int RTLD_LOCAL  = 0x000;
    //const int RTLD_LAZY   = 0x001;
    private const int RTLD_NOW = 0x002;

    [DllImport("c", EntryPoint = "mprotect", SetLastError = true)]
    internal static extern int MProtect(
      IntPtr addr,
      UIntPtr len,
      PROT prot
    );

    [DllImport("c", EntryPoint = "process_vm_readv", SetLastError = true)]
    internal static extern IntPtr ProcessVmReadV(
      int pid,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      iovec[] localIoVec,
      ulong localIovCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      iovec[] remoteIoVec,
      ulong remoteIoVecCount,
      ulong flags = 0);

    [DllImport("c", EntryPoint = "process_vm_writev", SetLastError = true)]
    internal static extern IntPtr ProcessVmWriteV(
      int pid,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      iovec localIoVec,
      ulong localIoVecCount,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
      iovec remoteIoVec,
      ulong remoteIoVecCount,
      ulong flags = 0);

    public static IntPtr LoadLibrary(string filename) {
      IntPtr h;

      try {
        h = DlV2.dlopen(filename, RTLD_NOW);
      }
      catch (DllNotFoundException) {
        h = DlV1.dlopen(filename, RTLD_NOW);
      }

      if (h != default)
        return h;

      string m;
      try {
        var p = DlV2.dlerror();
        m = p == default ? "Unknown error." : Marshal.PtrToStringAnsi(p);
      }
      catch (DllNotFoundException) {
        var p = DlV1.dlerror();
        m = p == default ? "Unknown error." : Marshal.PtrToStringAnsi(p);
      }

      throw new InvalidOperationException($"Error loading library {filename}", new Exception(m));
    }

    public static bool FreeLibrary(IntPtr module) {
      try {
        return DlV2.dlclose(module) == 0;
      }
      catch (DllNotFoundException) {
        return DlV1.dlclose(module) == 0;
      }
    }

    public static IntPtr GetProcAddress(IntPtr module, string method) {
      try {
        return DlV2.dlsym(module, method);
      }
      catch (DllNotFoundException) {
        return DlV1.dlsym(module, method);
      }
    }
    //const int RTLD_GLOBAL = 0x100;

    internal static class DlV1 {

      [DllImport("dl")]
      internal static extern IntPtr dlopen(string filename, int flags);

      [DllImport("dl")]
      internal static extern int dlclose(IntPtr module);

      [DllImport("dl")]
      internal static extern IntPtr dlsym(IntPtr handle, string symbol);

      [DllImport("dl")]
      internal static extern IntPtr dlerror();

    }

    internal static class DlV2 {

      [DllImport("libdl.so.2")]
      internal static extern IntPtr dlopen(string filename, int flags);

      [DllImport("libdl.so.2")]
      internal static extern int dlclose(IntPtr module);

      [DllImport("libdl.so.2")]
      internal static extern IntPtr dlsym(IntPtr handle, string symbol);

      [DllImport("libdl.so.2")]
      internal static extern IntPtr dlerror();

    }

  }

}