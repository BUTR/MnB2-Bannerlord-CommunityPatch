using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("07602928-CE38-4B83-81E7-74ADAF781214")]
  [ComImport]
  
  public interface ICorProfilerInfo5 : ICorProfilerInfo4 {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassFromObject(UIntPtr objectId, out UIntPtr pClassId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassFromToken(UIntPtr moduleId, [In] int typeDef, out UIntPtr pClassId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeInfo(UIntPtr functionId, [Out] IntPtr pStart, out uint pcSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetEventMask(out uint pdwEvents);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionFromIP([In] ref byte ip, out UIntPtr pFunctionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionFromToken(UIntPtr moduleId, [In] int token, out UIntPtr pFunctionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetHandleFromThread(UIntPtr threadId, out IntPtr phThread);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetObjectSize(UIntPtr objectId, out uint pcSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsArrayClass(UIntPtr classId, out uint pBaseElemType, out UIntPtr pBaseClassId, out uint pcRank);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadInfo(UIntPtr threadId, out uint pdwWin32ThreadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCurrentThreadID(out UIntPtr pThreadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassIDInfo(UIntPtr classId, out UIntPtr pModuleId, out int pTypeDefToken);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionInfo(UIntPtr functionId, out UIntPtr pClassId, out UIntPtr pModuleId, out int pToken);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEventMask([In] uint dwEvents);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionEnter(UIntPtr funcID);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionLeave(UIntPtr funcID);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionTailcall(UIntPtr funcID);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEnterLeaveFunctionHooks([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo pFuncEnter, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo pFuncLeave, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo pFuncTailcall);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        UIntPtr FunctionIDMapper(UIntPtr funcID, ref int pbHookFunction);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFunctionIDMapper([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo pFunc);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTokenAndMetaDataFromFunction(UIntPtr functionId, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppImport, out int pToken);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleInfo(UIntPtr moduleId, [Out] IntPtr ppBaseLoadAddress, [In] uint cchName, out uint pcchName, out ushort szName, out UIntPtr pAssemblyId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleMetaData(UIntPtr moduleId, [In] uint dwOpenFlags, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppOut);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILFunctionBody(UIntPtr moduleId, [In] int methodId, [Out] IntPtr ppMethodHeader, out uint pcbMethodSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILFunctionBodyAllocator(UIntPtr moduleId, [MarshalAs(UnmanagedType.Interface)] out IMethodMalloc ppMalloc);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILFunctionBody(UIntPtr moduleId, [In] int methodId, [In] ref byte pbNewILMethodHeader);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomainInfo(UIntPtr appDomainId, [In] uint cchName, out uint pcchName, out ushort szName, out UIntPtr pProcessId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssemblyInfo(UIntPtr assemblyId, [In] uint cchName, out uint pcchName, out ushort szName, out UIntPtr pAppDomainId, out UIntPtr pModuleId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFunctionReJIT(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ForceGC();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILInstrumentedCodeMap(UIntPtr functionId, [In] int fStartJit, [In] uint cILMapEntries, [In] ref COR_IL_MAP rgILMapEntries);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetInprocInspectionInterface([MarshalAs(UnmanagedType.IUnknown)] out object ppicd);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetInprocInspectionIThisThread([MarshalAs(UnmanagedType.IUnknown)] out object ppicd);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadContext(UIntPtr threadId, out UIntPtr pContextId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void BeginInprocDebugging([In] int fThisThreadOnly, out uint pdwProfilerContext);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EndInprocDebugging([In] uint dwProfilerContext);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILToNativeMapping(UIntPtr functionId, [In] uint cMap, out uint pcMap, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo map);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void StackSnapshotCallback(UIntPtr funcID, UIntPtr ip, UIntPtr frameInfo, uint contextSize, ref byte context, IntPtr clientData);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void DoStackSnapshot(UIntPtr thread, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo2 callback, [In] uint infoFlags, [In] IntPtr clientData, [In] ref byte context, [In] uint contextSize);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionEnter2(UIntPtr funcID, UIntPtr clientData, UIntPtr func, ref COR_PRF_FUNCTION_ARGUMENT_INFO argumentInfo);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionLeave2(UIntPtr funcID, UIntPtr clientData, UIntPtr func, ref COR_PRF_FUNCTION_ARGUMENT_RANGE retvalRange);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionTailcall2(UIntPtr funcID, UIntPtr clientData, UIntPtr func);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEnterLeaveFunctionHooks2([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo2 pFuncEnter, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo2 pFuncLeave, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo2 pFuncTailcall);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionInfo2(UIntPtr funcID, UIntPtr frameInfo, out UIntPtr pClassId, out UIntPtr pModuleId, out int pToken, [In] uint cTypeArgs, out uint pcTypeArgs, out UIntPtr typeArgs);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStringLayout(out uint pBufferLengthOffset, out uint pStringLengthOffset, out uint pBufferOffset);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassLayout(UIntPtr classId, [In] [Out] ref COR_FIELD_OFFSET rFieldOffset, [In] uint cFieldOffset, out uint pcFieldOffset, out uint pulClassSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassIDInfo2(UIntPtr classId, out UIntPtr pModuleId, out int pTypeDefToken, out UIntPtr pParentClassId, [In] uint cNumTypeArgs, out uint pcNumTypeArgs, out UIntPtr typeArgs);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeInfo2(UIntPtr functionId, [In] uint cCodeInfos, out uint pcCodeInfos, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo2 codeInfos);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetClassFromTokenAndTypeArgs(UIntPtr moduleId, [In] int typeDef, [In] uint cTypeArgs, ref UIntPtr typeArgs, out UIntPtr pClassId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionFromTokenAndTypeArgs(UIntPtr moduleId, [In] int funcDef, UIntPtr classId, [In] uint cTypeArgs, ref UIntPtr typeArgs, out UIntPtr pFunctionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumModuleFrozenObjects(UIntPtr moduleId, [MarshalAs(UnmanagedType.Interface)] out ICorProfilerObjectEnum ppEnum);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetArrayObjectInfo(UIntPtr objectId, [In] uint cDimensions, out uint pDimensionSizes, out int pDimensionLowerBounds, [Out] IntPtr ppData);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetBoxClassLayout(UIntPtr classId, out uint pBufferOffset);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadAppDomain(UIntPtr threadId, out UIntPtr pAppDomainId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRVAStaticAddress(UIntPtr classId, [In] int fieldToken, out IntPtr ppAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomainStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr appDomainId, out IntPtr ppAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr threadId, out IntPtr ppAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetContextStaticAddress(UIntPtr classId, [In] int fieldToken, UIntPtr contextId, out IntPtr ppAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStaticFieldInfo(UIntPtr classId, [In] int fieldToken, out COR_PRF_STATIC_TYPE pFieldInfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetGenerationBounds([In] uint cObjectRanges, out uint pcObjectRanges, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo2 ranges);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetObjectGeneration(UIntPtr objectId, out COR_PRF_GC_GENERATION_RANGE range);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetNotifiedExceptionClauseInfo(out COR_PRF_EX_CLAUSE_INFO pinfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumJITedFunctions([MarshalAs(UnmanagedType.Interface)] out ICorProfilerFunctionEnum ppEnum);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RequestProfilerDetach([In] uint dwExpectedCompletionMilliseconds);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        UIntPtr FunctionIDMapper2(UIntPtr funcID, IntPtr clientData, ref int pbHookFunction);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFunctionIDMapper2([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFunc, [In] IntPtr clientData);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetStringLayout2(out uint pStringLengthOffset, out uint pBufferOffset);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionEnter3(FunctionIDOrClientID FunctionIDOrClientID);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionLeave3(FunctionIDOrClientID FunctionIDOrClientID);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionTailcall3(FunctionIDOrClientID FunctionIDOrClientID);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEnterLeaveFunctionHooks3([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncEnter3, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncLeave3, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncTailcall3);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionEnter3WithInfo(FunctionIDOrClientID FunctionIDOrClientID, UIntPtr eltInfo);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionLeave3WithInfo(FunctionIDOrClientID FunctionIDOrClientID, UIntPtr eltInfo);
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    void FunctionTailcall3WithInfo(FunctionIDOrClientID FunctionIDOrClientID, UIntPtr eltInfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEnterLeaveFunctionHooks3WithInfo([MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncEnter3WithInfo, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncLeave3WithInfo, [MarshalAs(UnmanagedType.Interface)] [In] ICorProfilerInfo3 pFuncTailcall3WithInfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionEnter3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo, [In] [Out] ref uint pcbArgumentInfo, out COR_PRF_FUNCTION_ARGUMENT_INFO pArgumentInfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionLeave3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo, out COR_PRF_FUNCTION_ARGUMENT_RANGE pRetvalRange);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionTailcall3Info(UIntPtr functionId, UIntPtr eltInfo, out UIntPtr pFrameInfo);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumModules([MarshalAs(UnmanagedType.Interface)] out ICorProfilerModuleEnum ppEnum);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRuntimeInformation(out ushort pClrInstanceId, out COR_PRF_RUNTIME_TYPE pRuntimeType, out ushort pMajorVersion, out ushort pMinorVersion, out ushort pBuildNumber, out ushort pQFEVersion, [In] uint cchVersionString, out uint pcchVersionString, out ushort szVersionString);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThreadStaticAddress2(UIntPtr classId, [In] int fieldToken, UIntPtr appDomainId, UIntPtr threadId, out IntPtr ppAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomainsContainingModule(UIntPtr moduleId, [In] uint cAppDomainIds, out uint pcAppDomainIds, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo3 appDomainIds);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleInfo2(UIntPtr moduleId, [Out] IntPtr ppBaseLoadAddress, [In] uint cchName, out uint pcchName, out ushort szName, out UIntPtr pAssemblyId, out uint pdwModuleFlags);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumThreads([MarshalAs(UnmanagedType.Interface)] out ICorProfilerThreadEnum ppEnum);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void InitializeCurrentThread();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RequestReJIT([In] uint cFunctions, ref UIntPtr moduleIds, [In] ref int methodIds);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RequestRevert([In] uint cFunctions, ref UIntPtr moduleIds, [In] ref int methodIds, [MarshalAs(UnmanagedType.Error)] out int status);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeInfo3(UIntPtr functionId, UIntPtr rejitId, [In] uint cCodeInfos, out uint pcCodeInfos, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo4 codeInfos);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunctionFromIP2([In] ref byte ip, out UIntPtr pFunctionId, out UIntPtr pReJitId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetReJITIDs(UIntPtr functionId, [In] uint cReJitIds, out uint pcReJitIds, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo4 reJitIds);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILToNativeMapping2(UIntPtr functionId, UIntPtr rejitId, [In] uint cMap, out uint pcMap, [MarshalAs(UnmanagedType.Interface)] [Out] ICorProfilerInfo4 map);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumJITedFunctions2([MarshalAs(UnmanagedType.Interface)] out ICorProfilerFunctionEnum ppEnum);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetObjectSize2(UIntPtr objectId, out UIntPtr pcSize);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetEventMask2(out uint pdwEventsLow, out uint pdwEventsHigh);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetEventMask2([In] uint dwEventsLow, [In] uint dwEventsHigh);

  }

}