using System;
using System.Runtime.InteropServices;
using Antijank.Interop;

namespace Antijank {

  internal partial class WindowsNatives {

    [DllImport("kernel32")]
    private static extern bool VirtualProtectEx(
      IntPtr hProcess,
      IntPtr lpAddress,
      UIntPtr dwSize,
      uint flNewProtect,
      out uint lpflOldProtect
    );

    [DllImport("kernel32")]
    private static extern UIntPtr VirtualQuery(
      UIntPtr lpAddress,
      out MEMORY_BASIC_INFORMATION lpBuffer,
      UIntPtr dwLength
    );

    [DllImport("kernel32")]
    internal static extern int ReadProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      IntPtr lpBuffer,
      uint dwSize,
      out uint lpNumberOfBytesRead
    );

    [DllImport("kernel32")]
    public static extern int WriteProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      IntPtr lpBuffer,
      uint dwSize,
      out uint lpNumberOfBytesRead
    );

    [DllImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpFileName);

  }

}