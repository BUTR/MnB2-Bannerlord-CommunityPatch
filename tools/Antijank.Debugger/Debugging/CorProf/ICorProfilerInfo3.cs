using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("B555ED4F-452A-4E54-8B39-B5360BAD32A0")]
  [ComConversionLoss]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorProfilerInfo3 : ICorProfilerInfo2 {

    void EnumJITedFunctions([MarshalAs(UnmanagedType.Interface)] out ICorProfilerFunctionEnum ppEnum);

    void RequestProfilerDetach([In] uint dwExpectedCompletionMilliseconds);

    void SetFunctionIDMapper2([In] IntPtr pFunc, [In] IntPtr clientData);

    void GetStringLayout2(out uint pStringLengthOffset, out uint pBufferOffset);

    void SetEnterLeaveFunctionHooks3([In] IntPtr pFuncEnter3, [In] IntPtr pFuncLeave3, [In] IntPtr pFuncTailcall3);

    void SetEnterLeaveFunctionHooks3WithInfo([In] IntPtr pFuncEnter3WithInfo, [In] IntPtr pFuncLeave3WithInfo, [In] IntPtr pFuncTailcall3WithInfo);

    void GetFunctionEnter3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo,
      [In] [Out] ref uint pcbArgumentInfo, out COR_PRF_FUNCTION_ARGUMENT_INFO pArgumentInfo);

    void GetFunctionLeave3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo,
      out COR_PRF_FUNCTION_ARGUMENT_RANGE pRetvalRange);

    void GetFunctionTailcall3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo);

    void EnumModules([Out][MarshalAs(UnmanagedType.Interface)] out ICorProfilerModuleEnum ppEnum);

    void GetRuntimeInformation(out ushort pClrInstanceId, out COR_PRF_RUNTIME_TYPE pRuntimeType,
      out ushort pMajorVersion, out ushort pMinorVersion, out ushort pBuildNumber, out ushort pQFEVersion,
      [In] uint cchVersionString, out uint pcchVersionString,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      ref char[] szVersionString);

    void GetThreadStaticAddress2(UIntPtr classId, [In] int fieldToken, UIntPtr appDomainId, UIntPtr threadId,
      out IntPtr ppAddress);

    void GetAppDomainsContainingModule(UIntPtr moduleId, [In] uint cAppDomainIds, out uint pcAppDomainIds,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerInfo3 appDomainIds);

    void GetModuleInfo2(UIntPtr moduleId, [Out] IntPtr ppBaseLoadAddress, [In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      ref char[] szName, out UIntPtr pAssemblyId, out uint pdwModuleFlags);

  }

}