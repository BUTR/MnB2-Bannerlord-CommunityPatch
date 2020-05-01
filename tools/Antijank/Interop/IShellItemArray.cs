using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Antijank.Interop {

  [Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IShellItemArray {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void BindToHandler([MarshalAs(UnmanagedType.Interface), In]
      IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppvOut);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetPropertyStore([In] PropertyStoreFlags Flags, [In] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetPropertyDescriptionList([In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetAttributes([In] ShellItemAttributeFlags AttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCount(out uint pdwNumItems);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EnumItems([MarshalAs(UnmanagedType.Interface)] out IEnumShellItems ppenumShellItems);

  }

}