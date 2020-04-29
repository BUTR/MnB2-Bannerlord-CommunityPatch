using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [ComImport]
  [Guid("2EE06488-C0D4-42B1-B26D-F3795EF606FB")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComConversionLoss]
  
  public interface ICorDebugProcess3 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEnableCustomNotification(
      [In] [MarshalAs(UnmanagedType.Interface)]
      ICorDebugClass pClass,
      [In] int fOnOff);

  }

}