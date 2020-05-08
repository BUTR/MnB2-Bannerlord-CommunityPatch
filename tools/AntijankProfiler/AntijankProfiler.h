#pragma once

#include <cor.h>
#include <corprof.h>
#include <corhlpr.h>
#include <string>
#include <memory>
#include <unordered_set>
#include <unordered_map>
#include "hashes.h"

const GUID g_ClsIdAntijankProfiler = {0x204AEE3C, 0xCEE0, 0x43D7, {0xBF, 0xCA, 0x6B, 0x0A, 0xFB, 0x22, 0xF0, 0x9C}};

class AntijankProfiler final : public ICorProfilerCallback9 {
public:
    struct injection_t {
        mdTypeRef mdInjection;
        mdMemberRef mdDispatch;
        mdMemberRef mdPack[12];
        mdTypeSpec mdGenVar[12];
        mdMemberRef mdGenPack[12];
        mdMethodSpec mdGenPackSpec[12];
    };

private:
    LONG m_refCount;

    typedef std::unordered_map<const std::wstring, const injection_t*, insensitive_wstring_hash, insensitive_wstring_compare> injections_t;

    injections_t m_injections;

    void Report(HRESULT hr) const;

    static void Report(const IID &rIid);

public:

    struct ICorProfilerInfo8 *pInfo;

    static AntijankProfiler *Instance();

    AntijankProfiler();

    [[nodiscard]]
    const injection_t * GetInjections(const std::wstring &wszModule) const;

    STDMETHODIMP InitializeForAttach(IUnknown *pCorProfilerInfoUnk, void *pvClientData, UINT cbClientData) final;

    STDMETHODIMP Initialize(IUnknown *pICorProfilerInfoUnk) final;

    STDMETHODIMP Shutdown() final;

    STDMETHODIMP QueryInterface(REFIID rIid, void **ppInterface) final;

    STDMETHODIMP_(ULONG) AddRef() final;

    STDMETHODIMP_(ULONG) Release() final;

    //

    STDMETHODIMP ProfilerAttachComplete() final;

    STDMETHODIMP ProfilerDetachSucceeded() final { return S_OK; }

    STDMETHODIMP AppDomainCreationStarted(AppDomainID appDomainId) final { return S_OK; };

    STDMETHODIMP AppDomainCreationFinished(AppDomainID appDomainId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP AppDomainShutdownStarted(AppDomainID appDomainId) final { return S_OK; };

    STDMETHODIMP AppDomainShutdownFinished(AppDomainID appDomainId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP AssemblyLoadStarted(AssemblyID assemblyId) final;

    STDMETHODIMP AssemblyLoadFinished(AssemblyID assemblyId, HRESULT hrStatus) final;

    STDMETHODIMP AssemblyUnloadStarted(AssemblyID assemblyId) final { return S_OK; };

    STDMETHODIMP AssemblyUnloadFinished(AssemblyID assemblyId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP ModuleLoadStarted(ModuleID moduleId) final;

    STDMETHODIMP ModuleLoadFinished(ModuleID moduleId, HRESULT hrStatus) final;

    STDMETHODIMP ModuleUnloadStarted(ModuleID moduleId) final { return S_OK; };

    STDMETHODIMP ModuleUnloadFinished(ModuleID moduleId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP ModuleAttachedToAssembly(ModuleID moduleId, AssemblyID assemblyId) final;

    STDMETHODIMP ClassLoadStarted(ClassID classId) final { return S_OK; };

    STDMETHODIMP ClassLoadFinished(ClassID classId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP ClassUnloadStarted(ClassID classId) final { return S_OK; };

    STDMETHODIMP ClassUnloadFinished(ClassID classId, HRESULT hrStatus) final { return S_OK; };

    STDMETHODIMP FunctionUnloadStarted(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP JITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock) final;

    STDMETHODIMP JITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) final;

    STDMETHODIMP JITCachedFunctionSearchStarted(FunctionID functionId, BOOL *pbUseCachedFunction) final { return S_OK; };

    STDMETHODIMP JITCachedFunctionSearchFinished(FunctionID functionId, COR_PRF_JIT_CACHE result) final { return S_OK; };

    STDMETHODIMP JITFunctionPitched(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP JITInlining(FunctionID callerId, FunctionID calleeId, BOOL *pfShouldInline) final;

    STDMETHODIMP ThreadCreated(ThreadID threadId) final { return S_OK; };

    STDMETHODIMP ThreadDestroyed(ThreadID threadId) final { return S_OK; };

    STDMETHODIMP ThreadAssignedToOSThread(ThreadID managedThreadId, ULONG osThreadId) final { return S_OK; };

    STDMETHODIMP RemotingClientInvocationStarted() final { return S_OK; };

    STDMETHODIMP RemotingClientSendingMessage(GUID *pCookie, BOOL fIsAsync) final { return S_OK; };

    STDMETHODIMP RemotingClientReceivingReply(GUID *pCookie, BOOL fIsAsync) final { return S_OK; };

    STDMETHODIMP RemotingClientInvocationFinished() final { return S_OK; };

    STDMETHODIMP RemotingServerReceivingMessage(GUID *pCookie, BOOL fIsAsync) final { return S_OK; };

    STDMETHODIMP RemotingServerInvocationStarted() final { return S_OK; };

    STDMETHODIMP RemotingServerInvocationReturned() final { return S_OK; };

    STDMETHODIMP RemotingServerSendingReply(GUID *pCookie, BOOL fIsAsync) final { return S_OK; };

    STDMETHODIMP UnmanagedToManagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) final { return S_OK; };

    STDMETHODIMP ManagedToUnmanagedTransition(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) final { return S_OK; };

    STDMETHODIMP RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason) final { return S_OK; };

    STDMETHODIMP RuntimeSuspendFinished() final { return S_OK; };

    STDMETHODIMP RuntimeSuspendAborted() final { return S_OK; };

    STDMETHODIMP RuntimeResumeStarted() final { return S_OK; };

    STDMETHODIMP RuntimeResumeFinished() final { return S_OK; };

    STDMETHODIMP RuntimeThreadSuspended(ThreadID threadId) final { return S_OK; };

    STDMETHODIMP RuntimeThreadResumed(ThreadID threadId) final { return S_OK; };

    STDMETHODIMP MovedReferences(ULONG cMovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], ULONG cObjectIDRangeLength[]) final { return S_OK; };

    STDMETHODIMP ObjectAllocated(ObjectID objectId, ClassID classId) final { return S_OK; };

    STDMETHODIMP ObjectsAllocatedByClass(ULONG cClassCount, ClassID classIds[], ULONG cObjects[]) final { return S_OK; };

    STDMETHODIMP ObjectReferences(ObjectID objectId, ClassID classId, ULONG cObjectRefs, ObjectID objectRefIds[]) final { return S_OK; };

    STDMETHODIMP RootReferences(ULONG cRootRefs, ObjectID rootRefIds[]) final { return S_OK; }

    STDMETHODIMP ExceptionThrown(ObjectID thrownObjectId) final { return S_OK; };

    STDMETHODIMP ExceptionSearchFunctionEnter(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionSearchFunctionLeave() final { return S_OK; };

    STDMETHODIMP ExceptionSearchFilterEnter(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionSearchFilterLeave() final { return S_OK; };

    STDMETHODIMP ExceptionSearchCatcherFound(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionOSHandlerEnter(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionOSHandlerLeave(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionUnwindFunctionEnter(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionUnwindFunctionLeave() final { return S_OK; };

    STDMETHODIMP ExceptionUnwindFinallyEnter(FunctionID functionId) final { return S_OK; };

    STDMETHODIMP ExceptionUnwindFinallyLeave() final { return S_OK; };

    STDMETHODIMP ExceptionCatcherEnter(FunctionID functionId, ObjectID objectId) final { return S_OK; };

    STDMETHODIMP ExceptionCatcherLeave() final { return S_OK; };

    STDMETHODIMP COMClassicVTableCreated(ClassID wrappedClassId, REFGUID implementedIID, void *pVTable, ULONG cSlots) final { return S_OK; };

    STDMETHODIMP COMClassicVTableDestroyed(ClassID wrappedClassId, REFGUID implementedIID, void *pVTable) final { return S_OK; };

    STDMETHODIMP ExceptionCLRCatcherFound() final { return S_OK; };

    STDMETHODIMP ExceptionCLRCatcherExecute() final { return S_OK; };

    STDMETHODIMP ThreadNameChanged(ThreadID threadId, ULONG cchName, WCHAR *name) final { return S_OK; };

    STDMETHODIMP GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason) final { return S_OK; };

    STDMETHODIMP SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[]) final { return S_OK; };

    STDMETHODIMP GarbageCollectionFinished() final { return S_OK; };

    STDMETHODIMP FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID) final { return S_OK; };

    STDMETHODIMP RootReferences2(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]) final { return S_OK; };

    STDMETHODIMP HandleCreated(GCHandleID handleId, ObjectID initialObjectId) final { return S_OK; };

    STDMETHODIMP HandleDestroyed(GCHandleID handleId) final { return S_OK; }

    STDMETHODIMP ReJITCompilationStarted(FunctionID functionId, ReJITID rejitId, BOOL fIsSafeToBlock) final { return S_OK; }

    STDMETHODIMP GetReJITParameters(ModuleID moduleId, mdMethodDef methodId, ICorProfilerFunctionControl *pFunctionControl) final { return S_OK; }

    STDMETHODIMP ReJITCompilationFinished(FunctionID functionId, ReJITID rejitId, HRESULT hrStatus, BOOL fIsSafeToBlock) final { return S_OK; }

    STDMETHODIMP ReJITError(ModuleID moduleId, mdMethodDef methodId, FunctionID functionId, HRESULT hrStatus) final { return S_OK; }

    STDMETHODIMP MovedReferences2(ULONG cMovedObjectIDRanges, ObjectID *oldObjectIDRangeStart, ObjectID *newObjectIDRangeStart, SIZE_T *cObjectIDRangeLength) final { return S_OK; }

    STDMETHODIMP SurvivingReferences2(ULONG cSurvivingObjectIDRanges, ObjectID *objectIDRangeStart, SIZE_T *cObjectIDRangeLength) final { return S_OK; }

    STDMETHODIMP ConditionalWeakTableElementReferences(ULONG cRootRefs, ObjectID *keyRefIds, ObjectID *valueRefIds, GCHandleID *rootIds) final { return S_OK; }

    STDMETHODIMP GetAssemblyReferences(LPCWSTR wszAssemblyPath, ICorProfilerAssemblyReferenceProvider *pAsmRefProvider) final;

    STDMETHODIMP ModuleInMemorySymbolsUpdated(ModuleID moduleId) final { return S_OK; }

    STDMETHODIMP DynamicMethodJITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock, LPCBYTE pILHeader, ULONG cbILHeader) final { return S_OK; }

    STDMETHODIMP DynamicMethodJITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) final { return S_OK; }

    STDMETHODIMP DynamicMethodUnloaded(FunctionID functionId) final { return S_OK; }

};
