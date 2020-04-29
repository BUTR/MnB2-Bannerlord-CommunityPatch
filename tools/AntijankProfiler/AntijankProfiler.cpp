#include "AntijankProfiler.h"


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
