#include "Plumbing.h"

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved) {
    switch (dwReason) {
        case DLL_PROCESS_ATTACH:
            DisableThreadLibraryCalls(hInstance);     // Don't need the thread callbacks.
            break;
    }
    return TRUE;
}

STDAPI DllGetClassObject(const IID &rClsId, const IID &rIId, LPVOID *ppv) {
    if (rClsId == g_ClsIdAntijankProfiler) {
        // Create class factory.
        auto *pClassFactory = new AntijankProfilerClassFactory();
        return pClassFactory->QueryInterface(rIId, ppv);
    }
    return E_FAIL;
}

STDAPI_(ICorProfilerInfo3 *)GetCorProfilerInfo() {
    auto pProfiler = AntijankProfiler::Instance();
    if (pProfiler == nullptr)
        return nullptr;
    return pProfiler->pInfo;
}

STDAPI DllCanUnloadNow(void) { return S_OK; }
