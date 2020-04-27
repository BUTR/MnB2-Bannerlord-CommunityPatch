using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("11588775-7205-4CEB-A41A-93753C3153E9")]
  
  public interface ICorDebugProcess6 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppEvent")]
    ICorDebugDebugEvent DecodeEvent(
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] [In]
      byte[] pRecord,
      [In] uint countBytes,
      [In] CorDebugRecordFormat format,
      [In] uint dwFlags,
      [In] uint dwThreadId
    );

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ProcessStateChanged(CorDebugStateChange change);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppCode")]
    ICorDebugCode GetCode(ulong codeAddress);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableVirtualModuleSplitting([MarshalAs(UnmanagedType.Bool)] bool enableSplitting);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void MarkDebuggerAttached([MarshalAs(UnmanagedType.Bool)] bool fIsAttached);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pInvokePurpose")]
    CorDebugCodeInvokePurpose GetExportStepInfo(
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pszExportName,
      [Out] out CorDebugCodeInvokeKind pInvokeKind
    );

  }

}