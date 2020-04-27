using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Interop {

  
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [StructLayout(LayoutKind.Sequential)]
  public struct MEMORY_BASIC_INFORMATION {

    public IntPtr BaseAddress;

    public IntPtr AllocationBase;

    public uint AllocationProtect;

    public IntPtr RegionSize;

    public uint State;

    public uint Protect;

    public uint Type;

  }

}