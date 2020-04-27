using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ComImport]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E930C679-78AF-4953-8AB7-B0AABF0F9F80")]
  
  public interface ICorDebugProcess4 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Filter(
      [In] IntPtr pRecord,
      [In] uint countBytes,
      [In] CorDebugRecordFormat format,
      [In] CorDebugFilterFlagsWindows dwFlags,
      [In] uint dwThreadId,
      [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugManagedCallback pCallback,
      [In] [Out] ref uint dwContinueStatus);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ProcessStateChanged([In] CorDebugStateChange eChange);

  }

}