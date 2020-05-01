using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB")]
  [ComImport]
  public interface IFileSaveDialog : IFileDialog {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Show([In] IntPtr hwndOwner);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFileTypes([In] uint cFileTypes, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] COMDLG_FILTERSPEC[] rgFilterSpec);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFileTypeIndex([In] uint iFileType);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetFileTypeIndex(out uint piFileType);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Advise([MarshalAs(UnmanagedType.Interface), In]
      IFileDialogEvents pfde, out uint pdwCookie);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Unadvise([In] uint dwCookie);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetOptions([In] FileDialogOptions fos);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetOptions(out FileDialogOptions pfos);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetDefaultFolder([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFolder([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFileName([MarshalAs(UnmanagedType.LPWStr), In] string pszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetTitle([MarshalAs(UnmanagedType.LPWStr), In] string pszTitle);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr), In] string pszText);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr), In] string pszLabel);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void AddPlace([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, [In] FileDialogAddedPlace FDAP);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr), In] string pszDefaultExtension);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Close([MarshalAs(UnmanagedType.Error), In] int hr);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetClientGuid([In] ref Guid guid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void ClearClientData();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetFilter([MarshalAs(UnmanagedType.Interface), In]
      IShellItemFilter pFilter);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetSaveAsItem([MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetProperties([MarshalAs(UnmanagedType.Interface), In]
      IPropertyStore pStore);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetCollectedProperties([MarshalAs(UnmanagedType.Interface), In]
      IPropertyDescriptionList pList, [In] int fAppendDefault);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetProperties([MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppStore);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ApplyProperties(
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi,
      [MarshalAs(UnmanagedType.Interface), In]
      IPropertyStore pStore,
      [In] IntPtr hwnd,
      [MarshalAs(UnmanagedType.Interface), In]
      IFileOperationProgressSink pSink);

  }

}