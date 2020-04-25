using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("8A8CC829-CCF2-49FE-BBAE-0F022228071A")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorProfilerCallback2 : ICorProfilerCallback {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Initialize([MarshalAs(UnmanagedType.IUnknown)] [In] object pICorProfilerInfoUnk);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Shutdown();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AppDomainCreationStarted(UIntPtr appDomainId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AppDomainCreationFinished(UIntPtr appDomainId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AppDomainShutdownStarted(UIntPtr appDomainId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AppDomainShutdownFinished(UIntPtr appDomainId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AssemblyLoadStarted(UIntPtr assemblyId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AssemblyLoadFinished(UIntPtr assemblyId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AssemblyUnloadStarted(UIntPtr assemblyId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void AssemblyUnloadFinished(UIntPtr assemblyId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModuleLoadStarted(UIntPtr moduleId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModuleLoadFinished(UIntPtr moduleId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModuleUnloadStarted(UIntPtr moduleId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModuleUnloadFinished(UIntPtr moduleId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ModuleAttachedToAssembly(UIntPtr moduleId, UIntPtr assemblyId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClassLoadStarted(UIntPtr classId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClassLoadFinished(UIntPtr classId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClassUnloadStarted(UIntPtr classId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ClassUnloadFinished(UIntPtr classId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void FunctionUnloadStarted(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITCompilationStarted(UIntPtr functionId, [In] int fIsSafeToBlock);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITCompilationFinished(UIntPtr functionId, [MarshalAs(UnmanagedType.Error)] [In] int hrStatus, [In] int fIsSafeToBlock);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITCachedFunctionSearchStarted(UIntPtr functionId, out int pbUseCachedFunction);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITCachedFunctionSearchFinished(UIntPtr functionId, [In] UIntPtr result);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITFunctionPitched(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void JITInlining(UIntPtr callerId, UIntPtr calleeId, out int pfShouldInline);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ThreadCreated(UIntPtr threadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ThreadDestroyed(UIntPtr threadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ThreadAssignedToOSThread(UIntPtr managedThreadId, [In] uint osThreadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingClientInvocationStarted();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingClientSendingMessage([In] ref Guid pCookie, [In] int fIsAsync);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingClientReceivingReply([In] ref Guid pCookie, [In] int fIsAsync);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingClientInvocationFinished();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingServerReceivingMessage([In] ref Guid pCookie, [In] int fIsAsync);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingServerInvocationStarted();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingServerInvocationReturned();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RemotingServerSendingReply([In] ref Guid pCookie, [In] int fIsAsync);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void UnmanagedToManagedTransition(UIntPtr functionId, [In] UIntPtr reason);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ManagedToUnmanagedTransition(UIntPtr functionId, [In] UIntPtr reason);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeSuspendStarted([In] UIntPtr suspendReason);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeSuspendFinished();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeSuspendAborted();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeResumeStarted();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeResumeFinished();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeThreadSuspended(UIntPtr threadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RuntimeThreadResumed(UIntPtr threadId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void MovedReferences([In] uint cMovedObjectIDRanges, ref UIntPtr oldObjectIDRangeStart, ref UIntPtr newObjectIDRangeStart, [In] ref uint cObjectIDRangeLength);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ObjectAllocated(UIntPtr objectId, UIntPtr classId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ObjectsAllocatedByClass([In] uint cClassCount, ref UIntPtr classIds, [In] ref uint cObjects);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ObjectReferences(UIntPtr objectId, UIntPtr classId, [In] uint cObjectRefs, ref UIntPtr objectRefIds);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void RootReferences([In] uint cRootRefs, ref UIntPtr rootRefIds);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionThrown(UIntPtr thrownObjectId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionSearchFunctionEnter(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionSearchFunctionLeave();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionSearchFilterEnter(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionSearchFilterLeave();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionSearchCatcherFound(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionOSHandlerEnter(UIntPtr __unused);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionOSHandlerLeave(UIntPtr __unused);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionUnwindFunctionEnter(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionUnwindFunctionLeave();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionUnwindFinallyEnter(UIntPtr functionId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionUnwindFinallyLeave();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionCatcherEnter(UIntPtr functionId, UIntPtr objectId);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionCatcherLeave();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void COMClassicVTableCreated(UIntPtr wrappedClassId, [In] ref Guid implementedIID, [In] IntPtr pVTable, [In] uint cSlots);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void COMClassicVTableDestroyed(UIntPtr wrappedClassId, [In] ref Guid implementedIID, [In] IntPtr pVTable);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionCLRCatcherFound();
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ExceptionCLRCatcherExecute();
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void ThreadNameChanged(UIntPtr threadId, [In] uint cchName, [In] ref ushort name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GarbageCollectionStarted([In] int cGenerations, [In] ref int generationCollected, [In] UIntPtr reason);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SurvivingReferences([In] uint cSurvivingObjectIDRanges, ref UIntPtr objectIDRangeStart,
      [In] ref uint cObjectIDRangeLength);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GarbageCollectionFinished();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void FinalizeableObjectQueued([In] uint finalizerFlags, UIntPtr objectId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void RootReferences2([In] uint cRootRefs, ref UIntPtr rootRefIds, [In] ref COR_PRF_GC_ROOT_KIND rootKinds,
      [In] ref COR_PRF_GC_ROOT_FLAGS rootFlags, ref UIntPtr rootIds);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void HandleCreated(UIntPtr handleId, UIntPtr initialObjectId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void HandleDestroyed(UIntPtr handleId);

  }

}