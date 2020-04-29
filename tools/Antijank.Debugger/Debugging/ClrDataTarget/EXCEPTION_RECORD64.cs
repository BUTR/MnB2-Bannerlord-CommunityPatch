using System;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  
  public struct EXCEPTION_RECORD64 {

    public uint ExceptionCode;

    public uint ExceptionFlags;

    public ulong ExceptionRecord;

    public ulong ExceptionAddress;

    public uint NumberParameters;

    public uint __unusedAlignment;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public ulong[] ExceptionInformation;

  }

}