#include "AntijankProfilerClassFactory.h"

STDMETHODIMP AntijankProfilerClassFactory::QueryInterface(REFIID riid, void **ppInterface) {
    if (IID_IUnknown == riid)
        *ppInterface = static_cast<IUnknown *> ( this );
    else if (IID_IClassFactory == riid)
        *ppInterface = static_cast<IClassFactory *> ( this );
    else {
        *ppInterface = nullptr;
        return (E_NOTIMPL);
    }
    reinterpret_cast<IUnknown *>( *ppInterface )->AddRef();
    return (S_OK);
}

STDMETHODIMP AntijankProfilerClassFactory::CreateInstance(IUnknown *pUnkOuter, REFIID riid, void **ppInterface) {
    if (pUnkOuter != nullptr)
        return (CLASS_E_NOAGGREGATION);

    auto *pProfilerCallback = new AntijankProfiler();
    if (pProfilerCallback == nullptr)
        return E_OUTOFMEMORY;
    return pProfilerCallback->QueryInterface(riid, ppInterface);
}
