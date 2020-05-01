using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [TypeLibType(TypeLibTypeFlags.FCanCreate)]
  [ClassInterface(ClassInterfaceType.None)]
  [Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
  [ComImport]
  public class FileOpenDialogClass : FileOpenDialog {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Show([In] IntPtr hwndOwner);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFileTypes([In] uint cFileTypes, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In]
      COMDLG_FILTERSPEC[] rgFilterSpec);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFileTypeIndex([In] uint iFileType);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetFileTypeIndex(out uint piFileType);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Advise([MarshalAs(UnmanagedType.Interface), In]
      IFileDialogEvents pfde, out uint pdwCookie);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Unadvise([In] uint dwCookie);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetOptions([In] FileDialogOptions fos);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetOptions(out FileDialogOptions pfos);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetDefaultFolder([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFolder([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFileName([MarshalAs(UnmanagedType.LPWStr), In] string pszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetTitle([MarshalAs(UnmanagedType.LPWStr), In] string pszTitle);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr), In] string pszText);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr), In] string pszLabel);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void AddPlace([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, [In] FileDialogAddedPlace FDAP);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr), In] string pszDefaultExtension);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Close([MarshalAs(UnmanagedType.Error), In] int hr);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetClientGuid([In] ref Guid guid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void ClearClientData();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetFilter([MarshalAs(UnmanagedType.Interface), In]
      IShellItemFilter pFilter);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);

  }

}