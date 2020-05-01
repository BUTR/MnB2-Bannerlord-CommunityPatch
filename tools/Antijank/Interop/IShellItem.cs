using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Antijank.Interop {

  [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IShellItem {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void BindToHandler([MarshalAs(UnmanagedType.Interface), In]
      IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDisplayName([In] ShellItemDisplayName sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Compare([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, [In] uint hint, out int piOrder);

  }

}