#include "AntijankProfilerClassFactory.h"
#include <comdef.h>
#include <iostream>
#include <iomanip>

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
    HRESULT hr;
    if (FAILED(hr = pProfilerCallback->QueryInterface(riid, ppInterface))) {
        _com_error err(hr);
        LPCTSTR errMsg = err.ErrorMessage();
        std::wcout << L"HR 0x" << std::hex << std::setfill(L'0') << std::setw(8) << hr << L": " << errMsg << std::endl;
    }
    return hr;
}
