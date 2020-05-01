using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Guid("B4DB1657-70D7-485E-8E3E-6FCB5A5C1802")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IModalWindow {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Show([In] IntPtr hwndOwner);

  }

}