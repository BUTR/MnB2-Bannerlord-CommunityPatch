using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComConversionLoss]
  [Guid("28B5557D-3F3F-48B4-90B2-5F9EEA2F6C48")]
  [ComImport]
  public interface ICorProfilerInfo {

    void GetClassFromObject(UIntPtr objectId, out UIntPtr pClassId);

    void GetClassFromToken(UIntPtr moduleId, [In] int typeDef, out UIntPtr pClassId);

    void GetCodeInfo(UIntPtr functionId, [Out] IntPtr pStart, out uint pcSize);

    void GetEventMask(out uint pdwEvents);

    void GetFunctionFromIP([In] ref byte ip, out UIntPtr pFunctionId);

    void GetFunctionFromToken(UIntPtr moduleId, [In] int token, out UIntPtr pFunctionId);

    void GetHandleFromThread(UIntPtr threadId, out IntPtr phThread);

    void GetObjectSize(UIntPtr objectId, out uint pcSize);

    void IsArrayClass(UIntPtr classId, out uint pBaseElemType, out UIntPtr pBaseClassId, out uint pcRank);

    void GetThreadInfo(UIntPtr threadId, out uint pdwWin32ThreadId);

    void GetCurrentThreadID(out UIntPtr pThreadId);

    void GetClassIDInfo(UIntPtr classId, out UIntPtr pModuleId, out int pTypeDefToken);

    void GetFunctionInfo(UIntPtr functionId, out UIntPtr pClassId, out UIntPtr pModuleId, out int pToken);

    void SetEventMask([In] uint dwEvents);

    void SetEnterLeaveFunctionHooks([In] IntPtr pFuncEnter, [In] IntPtr pFuncLeave);

    void SetFunctionIDMapper([In] IntPtr pFunc);

    void GetTokenAndMetaDataFromFunction(IntPtr functionId, [In] ref Guid riid);

    void GetModuleInfo(UIntPtr moduleId, [Out] IntPtr ppBaseLoadAddress,
      [In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
      out char[] szName, out UIntPtr pAssemblyId);

    void GetModuleMetaData(UIntPtr moduleId, [In] uint dwOpenFlags, [In] ref Guid riid,
      [MarshalAs(UnmanagedType.IUnknown)] out object ppOut);

    void GetILFunctionBody(UIntPtr moduleId, [In] int methodId, [Out] IntPtr ppMethodHeader, out uint pcbMethodSize);

    void GetILFunctionBodyAllocator(UIntPtr moduleId, [MarshalAs(UnmanagedType.Interface)] out IMethodMalloc ppMalloc);

    void SetILFunctionBody(UIntPtr moduleId, [In] int methodId, [In] IntPtr pbNewILMethodHeader);

    void GetAppDomainInfo(UIntPtr appDomainId, [In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      char[] szName, out UIntPtr pProcessId);

    void GetAssemblyInfo(UIntPtr assemblyId, [In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      char[] szName, out UIntPtr pAppDomainId, out UIntPtr pModuleId);

    void SetFunctionReJIT(UIntPtr functionId);

    void ForceGC();

    void SetILInstrumentedCodeMap(UIntPtr functionId, [In] int fStartJit, [In] uint cILMapEntries,
      [In] COR_IL_MAP[] rgILMapEntries);

    void GetInprocInspectionInterface([MarshalAs(UnmanagedType.IUnknown)] out object ppicd);

    void GetInprocInspectionIThisThread([MarshalAs(UnmanagedType.IUnknown)] out object ppicd);

    void GetThreadContext(UIntPtr threadId, out UIntPtr pContextId);

    void BeginInprocDebugging([In] int fThisThreadOnly, out uint pdwProfilerContext);

    void EndInprocDebugging([In] uint dwProfilerContext);

    void GetILToNativeMapping(UIntPtr functionId,
      [In] uint cMap, out uint pcMap,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out]
      COR_DEBUG_IL_TO_NATIVE_MAP[] map);

  }

}