using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("7B63B2E3-107D-4D48-B2F6-F61E229470D2")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICorProfilerCallback4 : ICorProfilerCallback3 {

    void ReJITCompilationStarted(
      UIntPtr functionId,
      UIntPtr rejitId,
      [In] int fIsSafeToBlock);

    void GetReJITParameters(
      UIntPtr moduleId,
      [In] int methodId,
      [MarshalAs(UnmanagedType.Interface)] [In]
      ICorProfilerFunctionControl pFunctionControl);

    void ReJITCompilationFinished(
      UIntPtr functionId,
      UIntPtr rejitId,
      [MarshalAs(UnmanagedType.Error)] [In] int hrStatus,
      [In] int fIsSafeToBlock);

    void ReJITError(
      UIntPtr moduleId,
      [In] int methodId,
      UIntPtr functionId,
      [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);

    void MovedReferences2(
      [In] uint cMovedObjectIDRanges,
      [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      ref UIntPtr[] oldObjectIDRangeStart,
      [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      ref UIntPtr[] newObjectIDRangeStart,
      [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      ref UIntPtr[] cObjectIDRangeLength);

    void SurvivingReferences2(
      [In] uint cSurvivingObjectIDRanges,
      [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      ref UIntPtr[] objectIDRangeStart,
      [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      ref UIntPtr[] cObjectIDRangeLength);

  }

}