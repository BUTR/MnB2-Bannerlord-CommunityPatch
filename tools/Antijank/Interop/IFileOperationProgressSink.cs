using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antijank.Interop {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("04B0F1A7-9490-44BC-96E1-4296A31252E2")]
  [ComImport]
  public interface IFileOperationProgressSink {

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void StartOperations();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void FinishOperations([MarshalAs(UnmanagedType.Error), In] int hrResult);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PreRenameItem([In] uint dwFlags, [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PostRenameItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName,
      [MarshalAs(UnmanagedType.Error), In] int hrRename,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiNewlyCreated);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PreMoveItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PostMoveItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName,
      [MarshalAs(UnmanagedType.Error), In] int hrMove,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiNewlyCreated);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PreCopyItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PostCopyItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName,
      [MarshalAs(UnmanagedType.Error), In] int hrCopy,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiNewlyCreated);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PreDeleteItem([In] uint dwFlags, [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PostDeleteItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiItem,
      [MarshalAs(UnmanagedType.Error), In] int hrDelete,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiNewlyCreated);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PreNewItem([In] uint dwFlags, [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PostNewItem(
      [In] uint dwFlags,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiDestinationFolder,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszNewName,
      [MarshalAs(UnmanagedType.LPWStr), In] string pszTemplateName,
      [In] uint dwFileAttributes,
      [MarshalAs(UnmanagedType.Error), In] int hrNew,
      [MarshalAs(UnmanagedType.Interface), In]
      IShellItem psiNewItem);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UpdateProgress([In] uint iWorkTotal, [In] uint iWorkSoFar);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ResetTimer();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PauseTimer();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ResumeTimer();

  }

}