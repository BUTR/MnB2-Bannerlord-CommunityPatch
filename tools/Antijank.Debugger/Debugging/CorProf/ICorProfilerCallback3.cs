using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("4FD2ED52-7731-4B8D-9469-03D2CC3086C5")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorProfilerCallback3 : ICorProfilerCallback2 {

    /*
void Initialize([MarshalAs(UnmanagedType.IUnknown)] [In] object pICorProfilerInfoUnk);
void Shutdown();
void AppDomainCreationStarted(UIntPtr appDomainId);
void AppDomainCreationFinished(UIntPtr appDomainId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void AppDomainShutdownStarted(UIntPtr appDomainId);
void AppDomainShutdownFinished(UIntPtr appDomainId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void AssemblyLoadStarted(UIntPtr assemblyId);
void AssemblyLoadFinished(UIntPtr assemblyId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void AssemblyUnloadStarted(UIntPtr assemblyId);
void AssemblyUnloadFinished(UIntPtr assemblyId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void ModuleLoadStarted(UIntPtr moduleId);
void ModuleLoadFinished(UIntPtr moduleId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void ModuleUnloadStarted(UIntPtr moduleId);
void ModuleUnloadFinished(UIntPtr moduleId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void ModuleAttachedToAssembly(UIntPtr moduleId, UIntPtr assemblyId);
void ClassLoadStarted(UIntPtr classId);
void ClassLoadFinished(UIntPtr classId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void ClassUnloadStarted(UIntPtr classId);
void ClassUnloadFinished(UIntPtr classId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
void FunctionUnloadStarted(UIntPtr functionId);
void JITCompilationStarted(UIntPtr functionId, [In] int fIsSafeToBlock);
void JITCompilationFinished(UIntPtr functionId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus, [In] int fIsSafeToBlock);
void JITCachedFunctionSearchStarted(UIntPtr functionId, out int pbUseCachedFunction);
void JITCachedFunctionSearchFinished(UIntPtr functionId, [In] UIntPtr result);
void JITFunctionPitched(UIntPtr functionId);
void JITInlining(UIntPtr callerId, UIntPtr calleeId, out int pfShouldInline);
void ThreadCreated(UIntPtr threadId);
void ThreadDestroyed(UIntPtr threadId);
void ThreadAssignedToOSThread(UIntPtr managedThreadId, [In] uint osThreadId);
void RemotingClientInvocationStarted();
void RemotingClientSendingMessage([In] ref Guid pCookie, [In] int fIsAsync);
void RemotingClientReceivingReply([In] ref Guid pCookie, [In] int fIsAsync);
void RemotingClientInvocationFinished();
void RemotingServerReceivingMessage([In] ref Guid pCookie, [In] int fIsAsync);
void RemotingServerInvocationStarted();
void RemotingServerInvocationReturned();
void RemotingServerSendingReply([In] ref Guid pCookie, [In] int fIsAsync);
void UnmanagedToManagedTransition(UIntPtr functionId, [In] UIntPtr reason);
void ManagedToUnmanagedTransition(UIntPtr functionId, [In] UIntPtr reason);
void RuntimeSuspendStarted([In] UIntPtr suspendReason);
void RuntimeSuspendFinished();
void RuntimeSuspendAborted();
void RuntimeResumeStarted();
void RuntimeResumeFinished();
void RuntimeThreadSuspended(UIntPtr threadId);
void RuntimeThreadResumed(UIntPtr threadId);
void MovedReferences([In] uint cMovedObjectIDRanges, ref UIntPtr oldObjectIDRangeStart, ref UIntPtr newObjectIDRangeStart, [In] ref uint cObjectIDRangeLength);
void ObjectAllocated(UIntPtr objectId, UIntPtr classId);
void ObjectsAllocatedByClass([In] uint cClassCount, ref UIntPtr classIds, [In] ref uint cObjects);
void ObjectReferences(UIntPtr objectId, UIntPtr classId, [In] uint cObjectRefs, ref UIntPtr objectRefIds);
void RootReferences([In] uint cRootRefs, ref UIntPtr rootRefIds);
void ExceptionThrown(UIntPtr thrownObjectId);
void ExceptionSearchFunctionEnter(UIntPtr functionId);
void ExceptionSearchFunctionLeave();
void ExceptionSearchFilterEnter(UIntPtr functionId);
void ExceptionSearchFilterLeave();
void ExceptionSearchCatcherFound(UIntPtr functionId);
void ExceptionOSHandlerEnter(UIntPtr __unused);
void ExceptionOSHandlerLeave(UIntPtr __unused);
void ExceptionUnwindFunctionEnter(UIntPtr functionId);
void ExceptionUnwindFunctionLeave();
void ExceptionUnwindFinallyEnter(UIntPtr functionId);
void ExceptionUnwindFinallyLeave();
void ExceptionCatcherEnter(UIntPtr functionId, UIntPtr objectId);
void ExceptionCatcherLeave();
void COMClassicVTableCreated(UIntPtr wrappedClassId, [In] ref Guid implementedIID, [In] IntPtr pVTable, [In] uint cSlots);
void COMClassicVTableDestroyed(UIntPtr wrappedClassId, [In] ref Guid implementedIID, [In] IntPtr pVTable);
void ExceptionCLRCatcherFound();
void ExceptionCLRCatcherExecute();
void ThreadNameChanged(UIntPtr threadId, [In] uint cchName, [In] ref ushort name);
void GarbageCollectionStarted([In] int cGenerations, [In] ref int generationCollected, [In] UIntPtr reason);
void SurvivingReferences([In] uint cSurvivingObjectIDRanges, ref UIntPtr objectIDRangeStart, [In] ref uint cObjectIDRangeLength);
void GarbageCollectionFinished();
void FinalizeableObjectQueued([In] uint finalizerFlags, UIntPtr objectId);
void RootReferences2([In] uint cRootRefs, ref UIntPtr rootRefIds, [In] ref COR_PRF_GC_ROOT_KIND rootKinds, [In] ref COR_PRF_GC_ROOT_FLAGS rootFlags, ref UIntPtr rootIds);
void HandleCreated(UIntPtr handleId, UIntPtr initialObjectId);
void HandleDestroyed(UIntPtr handleId);
    */
    void InitializeForAttach([MarshalAs(UnmanagedType.IUnknown)] [In]
      ICorProfilerInfo pCorProfilerInfoUnk, [In] IntPtr pvClientData, [In] uint cbClientData);

    void ProfilerAttachComplete();

    void ProfilerDetachSucceeded();

  }

}