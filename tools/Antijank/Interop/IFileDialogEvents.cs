using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [Guid("973510DB-7D7F-452B-8975-74A85828D354")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IFileDialogEvents {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnFileOk([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnFolderChanging([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiFolder);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnFolderChange([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnSelectionChange([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnShareViolation(
      [MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi,
      out FileDialogEventResponse pResponse);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnTypeChange([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnOverwrite([MarshalAs(UnmanagedType.Interface), In]
      IFileDialog pfd,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psi, out FileDialogEventResponse pResponse);

  }

}