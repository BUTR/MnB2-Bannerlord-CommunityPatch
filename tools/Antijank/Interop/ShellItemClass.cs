using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Antijank.Interop {

  [Guid("9AC9FBE1-E0A2-4AD6-B4EE-E212013EA917")]
  [TypeLibType(TypeLibTypeFlags.FCanCreate)]
  [ClassInterface(ClassInterfaceType.None)]
  [ComImport]
  public class ShellItemClass : ShellItem {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void BindToHandler(
      [MarshalAs(UnmanagedType.Interface), In]
      IBindCtx pbc,
      [In] ref Guid bhid,
      [In] ref Guid riid,
      out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetDisplayName([In] ShellItemDisplayName sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Compare([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, [In] uint hint, out int piOrder);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetPropertyStore(
      [In] PropertyStoreFlags Flags,
      [In] ref Guid riid,
      out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetPropertyStoreWithCreateObject(
      [In] PropertyStoreFlags Flags,
      [MarshalAs(UnmanagedType.IUnknown), In]
      object punkCreateObject,
      [In] ref Guid riid,
      out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetPropertyStoreForKeys(
      [In] ref PropertyKey rgKeys,
      [In] uint cKeys,
      [In] PropertyStoreFlags Flags,
      [In] ref Guid riid,
      out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetPropertyDescriptionList(
      [In] ref PropertyKey keyType,
      [In] ref Guid riid,
      out IntPtr ppv);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Update([MarshalAs(UnmanagedType.Interface), In]
      IBindCtx pbc);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetProperty(
      [In] ref PropertyKey key,
      out PROPVARIANT ppropvar);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetCLSID([In] ref PropertyKey key, out Guid pclsid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetFileTime([In] ref PropertyKey key, out FILETIME pft);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetInt32([In] ref PropertyKey key, out int pi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetString([In] ref PropertyKey key, [MarshalAs(UnmanagedType.LPWStr)] out string ppsz);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetUInt32([In] ref PropertyKey key, out uint pui);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetUInt64([In] ref PropertyKey key, out ulong pull);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetBool([In] ref PropertyKey key, out int pf);

  }

}