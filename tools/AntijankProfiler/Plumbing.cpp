#include "Plumbing.h"
#include <iomanip>
#include <iostream>

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

STDAPI_(ICorProfilerInfo3 *) GetCorProfilerInfo() {
    auto pProfiler = AntijankProfiler::Instance();
    if (pProfiler == nullptr)
        return nullptr;
    return pProfiler->pInfo;
}

STDAPI_(bool) GetInjections(const wchar_t * wszModule, const AntijankProfiler::injection_t **pInjections) {
    const std::wstring moduleName(wszModule);

    std::wcout << L"GetInjections: " << moduleName << std::endl;
    auto pProfiler = AntijankProfiler::Instance();
    if (pProfiler == nullptr)
        return false;

    auto injections = pProfiler->GetInjections(moduleName);

    if (injections == nullptr) {
        std::wcout << L"No injections for " << moduleName << std::endl;
        return false;
    }

    *pInjections = injections;

    std::wcout << L"Returned injections for " << moduleName << std::endl;

    return true;
}

STDAPI DllCanUnloadNow(void) { return S_OK; }
