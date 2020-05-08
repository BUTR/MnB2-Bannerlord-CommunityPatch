using System.Runtime.InteropServices;

namespace Antijank.Interop
{
  [StructLayout(LayoutKind.Sequential, Pack = 8)]
  public struct COMDLG_FILTERSPEC
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszSpec;
  }
}
