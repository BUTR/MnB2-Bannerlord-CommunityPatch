using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Guid("2659B475-EEB8-48B7-8F07-B378810F48CF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IShellItemFilter {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void IncludeItem([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetEnumFlagsForItem([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, out uint pgrfFlags);

  }

}