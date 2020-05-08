#include "AntijankProfiler.h"
#include <iomanip>
#include <iostream>
#include <cstdio>
#include <string_view>
#include <filesystem>
#include <bitset>
#include <comdef.h>

static AntijankProfiler *s_pInstance;

AntijankProfiler::AntijankProfiler() {
    int argc;
    auto argv = CommandLineToArgvW(GetCommandLineW(), &argc);

    bool diag = false;
    std::wstring_view diagStr(L"/diag");
    for (int i = 0; i < argc; ++i) {
        if (std::wstring_view(argv[i]).compare(diagStr) == 0) {
            diag = true;
            break;
        }
    }

    if (diag) {
        AllocConsole();
        freopen_s((FILE **) stdout, "CONOUT$", "w", stderr);
        freopen_s((FILE **) stdout, "CONOUT$", "w", stdout);
        std::cerr.clear();
        std::clog.clear();
        std::cout.clear();
        HANDLE hConOut = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
        SetStdHandle(STD_ERROR_HANDLE, hConOut);
        SetStdHandle(STD_OUTPUT_HANDLE, hConOut);
        std::wcerr.clear();
        std::wclog.clear();
        std::wcout.clear();
        std::wcout << L"AntijankProfiler allocating console due to /diag flag." << std::endl;
    }

    std::ios::sync_with_stdio(false);

    std::wcout << L"AntijankProfiler constructed." << std::endl;
    pInfo = nullptr;
    m_refCount = 0;
    s_pInstance = this;
}

void AntijankProfiler::Report(HRESULT hr) const {
    _com_error err(hr);
    LPCTSTR errMsg = err.ErrorMessage();
    std::wcout << L"HR 0x" << std::hex << std::setfill(L'0') << std::setw(8) << hr << L": " << errMsg << std::endl;
}

HRESULT AntijankProfiler::InitializeForAttach(IUnknown *pCorProfilerInfoUnk, void *pvClientData, UINT cbClientData) {
    std::wcout << L"AntijankProfiler initializing for attachment." << std::endl;

    return Initialize(pCorProfilerInfoUnk);
}

STDMETHODIMP AntijankProfiler::Initialize(IUnknown *pICorProfilerInfoUnk) {
    std::wcout << L"AntijankProfiler initializing." << std::endl;

    HRESULT hr;
    if (FAILED(hr = pICorProfilerInfoUnk->QueryInterface(__uuidof(ICorProfilerInfo8), (void **) &pInfo))) {
        std::wcout << L"AntijankProfiler unable to access ICorProfilerInfo8." << std::endl;
        Report(hr);
        return S_OK;
    }

    const DWORD lowEventMask = COR_PRF_DISABLE_TRANSPARENCY_CHECKS_UNDER_FULL_TRUST
        | COR_PRF_MONITOR_MODULE_LOADS
        //| COR_PRF_MONITOR_ASSEMBLY_LOADS
        //| COR_PRF_MONITOR_JIT_COMPILATION
        | COR_PRF_ENABLE_REJIT;

    const DWORD highEventMask = COR_PRF_HIGH_ADD_ASSEMBLY_REFERENCES;

    if (FAILED(hr = pInfo->SetEventMask2( lowEventMask, highEventMask ))) {
        std::wcout << L"AntijankProfiler unable to enable events." << std::endl;
        Report(hr);
        return S_OK;
    }
    DWORD lowEventMaskCheck;
    DWORD highEventMaskCheck;
    if (FAILED(hr = pInfo->GetEventMask2( &lowEventMaskCheck, &highEventMaskCheck ))) {
        std::wcout << L"AntijankProfiler unable to confirm events enabled." << std::endl;
        Report(hr);
        return S_OK;
    }
    if (lowEventMask != lowEventMaskCheck) {
        std::wcout << L"Low event mask different! "
            << L"0b" << std::bitset<32>(lowEventMask)
            << L"wanted vs 0b" << std::bitset<32>(lowEventMaskCheck)
            << L" actual : 0b" << std::bitset<32>(lowEventMask ^ lowEventMaskCheck)
            << L" difference."
            << std::endl;
    }
    if (highEventMask != highEventMaskCheck) {
        std::wcout << L"High event mask different! "
            << L"0b" << std::bitset<32>(highEventMask)
            << L"wanted vs 0b" << std::bitset<32>(highEventMaskCheck)
            << L" actual : 0b" << std::bitset<32>(highEventMask ^ highEventMaskCheck)
            << L" difference."
            << std::endl;
    }
    std::wcout << L"AntijankProfiler enabled events." << std::endl;
    Report(hr);

    return S_OK;

}

STDMETHODIMP AntijankProfiler::ProfilerAttachComplete() {
    std::wcout << L"AntijankProfiler attach complete." << std::endl;
    return S_OK;
}


STDMETHODIMP AntijankProfiler::Shutdown() {
    std::wcout << L"AntijankProfiler shutting down." << std::endl;
    if (pInfo != nullptr)
        pInfo->Release();
    pInfo = nullptr;
    return S_OK;
}

#pragma warning( push )
#pragma warning( disable: 4068 )
#pragma clang diagnostic push
#pragma ide diagnostic ignored "bugprone-branch-clone"

void AntijankProfiler::Report(const IID &rIid) {
    std::wcout << std::hex
        << L'{' << std::setfill(L'0') << std::setw(8) << rIid.Data1
        << L'-' << std::setfill(L'0') << std::setw(4) << (int)rIid.Data2
        << L'-' << std::setfill(L'0') << std::setw(4) << (int)rIid.Data3
            << L'-';
    for (int i = 0; i < 2; ++i )
        std::wcout << std::setfill(L'0') << std::setw(2) << (int)rIid.Data4[i];
    std::wcout << L'-';
    for (int i = 2; i < 8; ++i )
        std::wcout << std::setfill(L'0') << std::setw(2) << (int)rIid.Data4[i];
    std::wcout << L'}';
}

STDMETHODIMP AntijankProfiler::QueryInterface(const IID &rIid, void **ppInterface) {
    std::wcout << L"AntijankProfiler interface queried: ";
    Report(rIid);
    std::wcout << std::endl;

    if (rIid == IID_IUnknown)
        *ppInterface = static_cast<ICorProfilerCallback *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback))
        *ppInterface = static_cast<ICorProfilerCallback *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback2))
        *ppInterface = static_cast<ICorProfilerCallback2 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback3))
        *ppInterface = static_cast<ICorProfilerCallback3 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback4))
        *ppInterface = static_cast<ICorProfilerCallback4 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback5))
        *ppInterface = static_cast<ICorProfilerCallback5 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback6))
        *ppInterface = static_cast<ICorProfilerCallback6 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback7))
        *ppInterface = static_cast<ICorProfilerCallback7 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback8))
        *ppInterface = static_cast<ICorProfilerCallback8 *>(this);
    else if (rIid == __uuidof(ICorProfilerCallback9))
        *ppInterface = static_cast<ICorProfilerCallback9 *>(this);
    else {
        *ppInterface = nullptr;
        std::wcout << L"Interface not supported!" << std::endl;
        return E_NOTIMPL;
    }

    std::wcout << L"Interface supported." << std::endl;
    reinterpret_cast<IUnknown *>(*ppInterface)->AddRef();
    return S_OK;
}

#pragma clang diagnostic pop
#pragma warning( pop )

ULONG AntijankProfiler::AddRef() {
    std::wcout << L"AntijankProfiler Instance referenced." << std::endl;
    return InterlockedIncrement(&m_refCount);
}

ULONG AntijankProfiler::Release() {
    std::wcout << L"AntijankProfiler Instance reference released." << std::endl;
    auto ret = InterlockedDecrement(&m_refCount);
    if (ret <= 0) delete this;
    return ret;
}

AntijankProfiler *AntijankProfiler::Instance() {
    std::wcout << L"AntijankProfiler Instance fetched." << std::endl;
    return s_pInstance;
}

STDMETHODIMP AntijankProfiler::AssemblyLoadStarted(AssemblyID assemblyId) {
    std::wcout << L"AssemblyLoadStarted: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << assemblyId << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::AssemblyLoadFinished(AssemblyID assemblyId, HRESULT hrStatus) {
    std::wcout << L"AssemblyLoadFinished: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << assemblyId << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::ModuleLoadStarted(ModuleID moduleId) {
    std::wcout << L"ModuleLoadStarted: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << moduleId << std::endl;
    return S_OK;
}


STDMETHODIMP AntijankProfiler::GetAssemblyReferences(LPCWSTR wszAssemblyPath, ICorProfilerAssemblyReferenceProvider *pAsmRefProvider) {
    std::wcout << L"GetAssemblyReferences: " << wszAssemblyPath << std::endl;
    std::wstring_view wsvAssemblyPath( wszAssemblyPath );
    std::filesystem::path pathAssembly( wszAssemblyPath, std::filesystem::path::format::native_format );
    auto assemblyFileName = pathAssembly.filename().replace_extension();
    std::wcout << L"GetAssemblyReferences: " << assemblyFileName << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::ModuleLoadFinished(ModuleID moduleId, HRESULT hrStatus) {
    std::wcout << L"ModuleLoadFinished: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << moduleId << std::endl;

    HRESULT hr;

    WCHAR wcsBuf[512];
    ULONG cchBufUsed;
    LPCBYTE baseAddress;
    AssemblyID AsmId;
    DWORD moduleFlags;

    if (FAILED(hr = pInfo->GetModuleInfo2(moduleId, &baseAddress, 512, &cchBufUsed, &wcsBuf[0], &AsmId, &moduleFlags))) {
        std::wcout << L"ModuleLoadFinished: GetModuleInfo failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    if ((moduleFlags & COR_PRF_MODULE_DYNAMIC) != 0) {
        std::wcout << L"ModuleLoadFinished: Skipping dynamic module." << std::endl;
        return S_OK;
    }

    const std::wstring_view wsvAssemblyPath(wcsBuf, cchBufUsed);
    const std::filesystem::path pathAssembly(wsvAssemblyPath, std::filesystem::path::format::native_format);
    const std::wstring assemblyFileName(pathAssembly.filename().c_str());

    if (assemblyFileName.compare(L"mscorlib.dll") == 0 || assemblyFileName.compare(L"netstandard.dll") == 0) {
        std::wcout << L"ModuleLoadFinished: Explicitly skipping " << assemblyFileName << std::endl;
        return S_OK;
    }
    if (assemblyFileName.find(L"Harmony") != -1) {
        std::wcout << L"ModuleLoadFinished: Skipping Harmony library " << assemblyFileName << std::endl;
        return S_OK;
    }
    if (assemblyFileName.starts_with(L"System.")) {
        std::wcout << L"ModuleLoadFinished: Skipping System library " << assemblyFileName << std::endl;
        return S_OK;
    }
    if (assemblyFileName.starts_with(L"Antijank.")) {
        std::wcout << L"ModuleLoadFinished: Skipping Antijank library " << assemblyFileName << std::endl;
        return S_OK;
    }
    if (assemblyFileName.starts_with(L"MonoMod.")) {
        std::wcout << L"ModuleLoadFinished: Skipping MonoMod library " << assemblyFileName << std::endl;
        return S_OK;
    }

    std::wcout << L"AntijankProfiler: Adding Antijank.Injection.Dispatch reference to " << assemblyFileName << std::endl;


    IMetaDataEmit *pModMeta;

    if (FAILED(hr = pInfo->GetModuleMetaData(moduleId, ofRead | ofWrite, IID_IMetaDataEmit, (IUnknown **) &pModMeta))) {
        std::wcout << L"ModuleLoadFinished: GetModuleMetaData failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    IMetaDataAssemblyEmit *pModMetaAsm;

    if (FAILED(hr = pModMeta->QueryInterface(IID_IMetaDataAssemblyEmit, (void **) &pModMetaAsm))) {
        std::wcout << L"ModuleLoadFinished: querying IMetaDataAssemblyEmit failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    const DWORD proc8664 = PROCESSOR_AMD_X8664;
    const WCHAR wszLocale[] = L"neutral";
    const ASSEMBLYMETADATA asmRefMeta = {1, 0, 0, 0, nullptr, 0, (DWORD * ) & proc8664, 1, nullptr, 0};

    mdAssemblyRef mdAsmRef;
    if (FAILED(hr = pModMetaAsm->DefineAssemblyRef(nullptr, 0, L"Antijank", &asmRefMeta, nullptr, 0, afPA_AMD64, &mdAsmRef))) {
        std::wcout << L"ModuleLoadFinished: DefineAssemblyRef failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    IMetaDataEmit2 *pModMeta2;

    if (FAILED(hr = pModMeta->QueryInterface(IID_IMetaDataEmit2, (void **) &pModMeta2))) {
        std::wcout << L"ModuleLoadFinished: IMetaDataEmit2 failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    auto * injection = new injection_t;

    if (FAILED(hr = pModMeta->DefineTypeRefByName(mdAsmRef, L"Antijank.Injections", &injection->mdInjection))) {
        std::wcout << L"ModuleLoadFinished: DefineTypeRefByName failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    {
        const static BYTE sig[] = {
                IMAGE_CEE_CS_CALLCONV_DEFAULT, 2, // non-sentinel arg count
                ELEMENT_TYPE_VOID, // return type
                ELEMENT_TYPE_I4, // mdtoken
                ELEMENT_TYPE_OBJECT, // object
        };

        if (FAILED(hr = pModMeta->DefineMemberRef(injection->mdInjection, L"Dispatch", &sig[0], sizeof(sig), &injection->mdDispatch))) {
            std::wcout << L"ModuleLoadFinished: DefineMemberRef failed." << std::endl;
            Report(hr);
            return S_OK;
        }
    }

    {
        static BYTE sig[] = {
                IMAGE_CEE_CS_CALLCONV_DEFAULT, 1, // non-sentinel arg count
                ELEMENT_TYPE_OBJECT, // return type
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
                ELEMENT_TYPE_OBJECT, // object
        };

        for (int i = 1; i <= 12; ++i) {
            sig[1] = i;
            if (FAILED(hr = pModMeta->DefineMemberRef(injection->mdInjection, L"Pack", &sig[0], 3 + i, &injection->mdPack[i-1]))) {
                std::wcout << L"ModuleLoadFinished: DefineMemberRef failed." << std::endl;
                Report(hr);
                return S_OK;
            }
        }
    }

    {
        static BYTE sig[] = {
                IMAGE_CEE_CS_CALLCONV_GENERIC,
                1, // generic arg count
                1, // non-sentinel arg count
                ELEMENT_TYPE_OBJECT, // return type
                ELEMENT_TYPE_MVAR, 0, // object
                ELEMENT_TYPE_MVAR, 1, // object
                ELEMENT_TYPE_MVAR, 2, // object
                ELEMENT_TYPE_MVAR, 3, // object
                ELEMENT_TYPE_MVAR, 4, // object
                ELEMENT_TYPE_MVAR, 5, // object
                ELEMENT_TYPE_MVAR, 6, // object
                ELEMENT_TYPE_MVAR, 7, // object
                ELEMENT_TYPE_MVAR, 8, // object
                ELEMENT_TYPE_MVAR, 9, // object
                ELEMENT_TYPE_MVAR, 10, // object
                ELEMENT_TYPE_MVAR, 11, // object
        };

        //std::wstring wszGenPackBase(L"Pack`");
        for (int i = 1; i <= 12; ++i) {
            sig[1] = i;
            sig[2] = i;
            //auto wszGenPack = wszGenPackBase.append(std::to_wstring(i));
            if (FAILED(hr = pModMeta->DefineMemberRef(injection->mdInjection, L"Pack", &sig[0], 4 + i*2, &injection->mdGenPack[i-1]))) {
                std::wcout << L"ModuleLoadFinished: DefineMemberRef failed." << std::endl;
                Report(hr);
                return S_OK;
            }
        }
    }

    {
        static BYTE sig[] = {
                IMAGE_CEE_CS_CALLCONV_GENERICINST,
                1, // arg count
                ELEMENT_TYPE_VAR, 0, // object
                ELEMENT_TYPE_VAR, 1, // object
                ELEMENT_TYPE_VAR, 2, // object
                ELEMENT_TYPE_VAR, 3, // object
                ELEMENT_TYPE_VAR, 4, // object
                ELEMENT_TYPE_VAR, 5, // object
                ELEMENT_TYPE_VAR, 6, // object
                ELEMENT_TYPE_VAR, 7, // object
                ELEMENT_TYPE_VAR, 8, // object
                ELEMENT_TYPE_VAR, 9, // object
                ELEMENT_TYPE_VAR, 10, // object
                ELEMENT_TYPE_VAR, 11, // object
        };

        for (int i = 1; i <= 12; ++i) {
            sig[1] = i;
            if (FAILED(hr = pModMeta2->DefineMethodSpec(injection->mdGenPack[i-1], &sig[0], 2 + i*2, &injection->mdGenPackSpec[i-1]))) {
                std::wcout << L"ModuleLoadFinished: DefineMethodSpec failed." << std::endl;
                Report(hr);
                return S_OK;
            }
        }
    }

    {
        static BYTE sig[] = {
                ELEMENT_TYPE_VAR, // return type
                0
        };

        for (int i = 0; i < 12; ++i ) {
            sig[1] = i;
            pModMeta2->GetTokenFromTypeSpec(sig, 2, &injection->mdGenVar[i]);
        }

    }

    if (FAILED(hr = pInfo->ApplyMetaData(moduleId))) {
        std::wcout << L"ModuleLoadFinished: ApplyMetaData failed." << std::endl;
        Report(hr);
        return S_OK;
    }

    m_injections[std::wstring(pathAssembly.filename().c_str())] = injection;

    std::wcout << L"AntijankProfiler: Successfully added Antijank.Injection.Dispatch reference to " << assemblyFileName << std::endl
            << std::hex
            << L"Antijank.Injection TypeRef: 0x" << std::setfill(L'0') << std::setw(8) << injection->mdInjection
            << L" Antijank.Injection.Dispatch MemberRef: 0x" << std::setfill(L'0') << std::setw(8) << injection->mdDispatch
            << L" Pack MemberRefs:";
    for (unsigned mdPack : injection->mdPack) {
        std::wcout << L" 0x" << std::setfill(L'0') << std::setw(8) << mdPack;
    }

    std::wcout << L" Generic Var TypeSpecs:";
    for (unsigned mdGenVar : injection->mdGenVar) {
        std::wcout << L" 0x" << std::setfill(L'0') << std::setw(8) << mdGenVar;
    }

    std::wcout << L" Generic Pack MemberRefs:";
    for (unsigned mdGenPack : injection->mdGenPack) {
        std::wcout << L" 0x" << std::setfill(L'0') << std::setw(8) << mdGenPack;
    }

    std::wcout << L" Generic Pack MethodSpecs:";
    for (unsigned mdGenPackSpec : injection->mdGenPackSpec) {
        std::wcout << L" 0x" << std::setfill(L'0') << std::setw(8) << mdGenPackSpec;
    }
    std::wcout << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::ModuleAttachedToAssembly(ModuleID moduleId, AssemblyID assemblyId) {
    std::wcout << L"ModuleAttachedToAssembly: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << moduleId << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::JITCompilationStarted(FunctionID functionId, BOOL fIsSafeToBlock) {
    //std::wcout << "JITCompilationStarted: 0x" << std::hex << std::setfill(L'0') << std::setw(16) << functionId << std::endl;
    return S_OK;
}

STDMETHODIMP AntijankProfiler::JITCompilationFinished(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) {
    return S_OK;
}

STDMETHODIMP AntijankProfiler::JITInlining(FunctionID callerId, FunctionID calleeId, BOOL *pfShouldInline) {
    return S_OK;
}

const AntijankProfiler::injection_t * AntijankProfiler::GetInjections(const std::wstring &wszModule) const {
    std::wcout << L"Finding injection for " << wszModule << std::endl;
    auto value = m_injections.find(wszModule);
    if (value == m_injections.end()) {
        std::wcout << L"Missing injection for " << wszModule << std::endl;
        return nullptr;
    }
    std::wcout << L"Found injection for " << wszModule << std::endl;
    return value->second;
}
