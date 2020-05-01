#include "AntijankProfiler.h"
#include <iomanip>
#include <iostream>
#include <string_view>

static AntijankProfiler *s_pInstance;


AntijankProfiler::AntijankProfiler() {
    pInfo = nullptr;
    m_refCount = 0;
    s_pInstance = this;
}

STDMETHODIMP AntijankProfiler::InitializeForAttach(IUnknown *pCorProfilerInfoUnk, void *pvClientData, UINT cbClientData) {
    pCorProfilerInfoUnk->QueryInterface(__uuidof(ICorProfilerInfo), (void **) &pInfo);
    return 0;
}

STDMETHODIMP AntijankProfiler::Initialize(IUnknown *pICorProfilerInfoUnk) {
    return InitializeForAttach(pICorProfilerInfoUnk, nullptr, -1);
}


STDMETHODIMP AntijankProfiler::Shutdown() {
    if (pInfo != nullptr)
        pInfo->Release();
    pInfo = nullptr;
    return S_OK;
}

#pragma warning( push )
#pragma warning( disable: 4068 )
#pragma clang diagnostic push
#pragma ide diagnostic ignored "bugprone-branch-clone"

STDMETHODIMP AntijankProfiler::QueryInterface(const IID &rIid, void **ppInterface) {

    if (rIid == IID_IUnknown)
        *ppInterface = static_cast<ICorProfilerCallback *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback))
        *ppInterface = static_cast<ICorProfilerCallback *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback2))
        *ppInterface = static_cast<ICorProfilerCallback2 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback3))
        *ppInterface = static_cast<ICorProfilerCallback3 *>(this);
    else {
        *ppInterface = nullptr;
        return E_NOTIMPL;
    }
    reinterpret_cast<IUnknown *>(*ppInterface)->AddRef();
    return S_OK;
}

#pragma clang diagnostic pop
#pragma warning( pop )

ULONG AntijankProfiler::AddRef() {
    return InterlockedIncrement(&m_refCount);
}

ULONG AntijankProfiler::Release() {
    auto ret = InterlockedDecrement(&m_refCount);
    if (ret <= 0) delete this;
    return ret;
}

AntijankProfiler *AntijankProfiler::Instance() {
    return s_pInstance;
}

template<int wszModuleSize>
bool AntijankProfiler::GetMethodInfo(FunctionID functionId, ULONG &wszModuleLength, WCHAR (&wszModule)[wszModuleSize], mdToken &mdMethod) const {
    IMetaDataImport *pIMetaDataImport = nullptr;
    if (FAILED(pInfo->GetTokenAndMetaDataFromFunction(functionId, IID_IMetaDataImport, (IUnknown **) &pIMetaDataImport, &mdMethod)))
        return false;

    mdModule mdModule;

    if (FAILED(pIMetaDataImport->GetModuleFromScope(&mdModule)))
        return false;

    if (FAILED(pIMetaDataImport->GetModuleRefProps(mdModule, (LPWSTR) &wszModule, wszModuleSize, &wszModuleLength)))
        return false;

    pIMetaDataImport->Release();
    return true;
}

STDMETHODIMP AntijankProfiler::JITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock) {
    HRESULT hr = S_OK;

    mdToken mdMethod = 0;
    wchar_t wszModule[1024];
    ULONG wszModuleLength = 0;

    if (!GetMethodInfo(functionId, wszModuleLength, wszModule, mdMethod))
        return S_OK;

    auto moduleName = std::wstring_view(&wszModule[0], wszModuleLength);

    auto matchedModuleName = m_replacement_module_names.find(moduleName);

    // no module name reference found
    if (matchedModuleName == m_replacement_module_names.end())
        return S_OK;

    auto replacement = m_replacements.find(replacement_key_t(*matchedModuleName->second, mdMethod));

    // no replacement to be made
    if (replacement == m_replacements.end())
        return S_OK;

    ClassID classId = 0;
    ModuleID moduleId = 0;

    std::wcout << "Replacement: " << moduleName << "!0x" << std::hex << std::setfill(L'0') << std::setw(8) << mdMethod << std::endl;

    if (FAILED(pInfo->SetILFunctionBody(moduleId, mdMethod, replacement->second))) {
        std::wcout << "Replacement failed!";
        return S_OK;
    }

    std::wcout << "Replacement succeeded!";
    return S_OK;
}

