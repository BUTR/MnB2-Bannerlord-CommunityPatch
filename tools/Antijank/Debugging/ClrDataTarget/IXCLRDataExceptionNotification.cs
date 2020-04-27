using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("2D95A079-42A1-4837-818F-0B97D7048E0E")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface IXCLRDataExceptionNotification {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnCodeGenerated([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnCodeDiscarded([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataMethodInstance method);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnProcessExecution([In] uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnTaskExecution([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTask task, [In] uint state);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnModuleLoaded([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnModuleUnloaded([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataModule mod);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnTypeLoaded([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance typeInst);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OnTypeUnloaded([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDataTypeInstance typeInst);

  }

}