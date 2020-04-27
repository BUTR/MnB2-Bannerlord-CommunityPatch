using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComConversionLoss]
  [Guid("28B5557D-3F3F-48B4-90B2-5F9EEA2F6C48")]
  [ComImport]
  
  public interface ICorProfilerInfo {

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
    void SetEnterLeaveFunctionHooks([MarshalAs(UnmanagedType.Interface)] [In]
      ICorProfilerInfo pFuncEnter, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorProfilerInfo pFuncLeave, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorProfilerInfo pFuncTailcall);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr FunctionIDMapper(UIntPtr funcID, ref int pbHookFunction);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFunctionIDMapper([MarshalAs(UnmanagedType.Interface)] [In]
      ICorProfilerInfo pFunc);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetTokenAndMetaDataFromFunction(UIntPtr functionId, [In] ref Guid riid,
      [MarshalAs(UnmanagedType.IUnknown)] out object ppImport, out int pToken);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleInfo(UIntPtr moduleId, [Out] IntPtr ppBaseLoadAddress, [In] uint cchName, out uint pcchName,
      out ushort szName, out UIntPtr pAssemblyId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetModuleMetaData(UIntPtr moduleId, [In] uint dwOpenFlags, [In] ref Guid riid,
      [MarshalAs(UnmanagedType.IUnknown)] out object ppOut);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILFunctionBody(UIntPtr moduleId, [In] int methodId, [Out] IntPtr ppMethodHeader, out uint pcbMethodSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILFunctionBodyAllocator(UIntPtr moduleId, [MarshalAs(UnmanagedType.Interface)] out IMethodMalloc ppMalloc);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILFunctionBody(UIntPtr moduleId, [In] int methodId, [In] ref byte pbNewILMethodHeader);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAppDomainInfo(UIntPtr appDomainId, [In] uint cchName, out uint pcchName, out ushort szName,
      out UIntPtr pProcessId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAssemblyInfo(UIntPtr assemblyId, [In] uint cchName, out uint pcchName, out ushort szName,
      out UIntPtr pAppDomainId, out UIntPtr pModuleId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFunctionReJIT(UIntPtr functionId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void ForceGC();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetILInstrumentedCodeMap(UIntPtr functionId, [In] int fStartJit, [In] uint cILMapEntries,
      [In] ref COR_IL_MAP rgILMapEntries);

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
    void GetILToNativeMapping(UIntPtr functionId, [In] uint cMap, out uint pcMap,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerInfo map);

  }

}