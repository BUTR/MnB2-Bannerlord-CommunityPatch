#pragma once

#include "AntijankProfiler.h"

class AntijankProfilerClassFactory final : public IClassFactory {
public:
    AntijankProfilerClassFactory() {
        m_refCount = 0;
    }

    STDMETHODIMP_(ULONG) AddRef() final {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHODIMP_(ULONG) Release() final {
        auto ret = InterlockedDecrement(&m_refCount);
        if (ret <= 0) delete this;
        return ret;
    }

    STDMETHODIMP QueryInterface(REFIID riid, void **ppInterface) final;

    STDMETHODIMP LockServer(BOOL bLock) final { return S_OK; }

    STDMETHODIMP CreateInstance(IUnknown *pUnkOuter, REFIID riid, void **ppInterface) final;

private:
    long m_refCount;
};

