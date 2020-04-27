using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("63DF8730-DC81-4062-84A2-1FF943F59FDD")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICLRValidator {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Validate([MarshalAs(UnmanagedType.Interface)] [In]
      IVEHandler veh, [In] uint ulAppDomainId, [In] uint ulFlags, [In] uint ulMaxError, [In] uint Token,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string fileName, [In] ref byte pe, [In] uint ulSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void FormatEventInfo([MarshalAs(UnmanagedType.Error)] [In] int hVECode, [In] VerError Context,
      [MarshalAs(UnmanagedType.LPWStr)] [In] [Out]
      StringBuilder msg, [In] uint ulMaxLength,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] [In]
      object[] psa);

  }

}