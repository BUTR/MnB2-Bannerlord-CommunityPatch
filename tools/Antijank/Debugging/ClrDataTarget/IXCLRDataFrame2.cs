using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("1C4D9A4B-702D-4CF6-B290-1DB6F43050D0")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface IXCLRDataFrame2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetExactGenericArgsToken([MarshalAs(UnmanagedType.Interface)] out IXCLRDataValue genericToken);

  }

}