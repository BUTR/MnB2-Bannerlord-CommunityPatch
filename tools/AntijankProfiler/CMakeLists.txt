cmake_minimum_required(VERSION 3.16)
cmake_policy(SET CMP0069 NEW)
cmake_policy(SET CMP0091 NEW)

foreach (_config MINSIZEREL RELWITHDEBINFO RELEASE)
    set(CMAKE_C_FLAGS_${_config} "${CMAKE_C_FLAGS_${_config}} /GL")
    set(CMAKE_CXX_FLAGS_${_config} "${CMAKE_CXX_FLAGS_${_config}} /GL")
    set(CMAKE_EXE_LINKER_FLAGS_${_config}  "${CMAKE_EXE_LINKER_FLAGS_${_config}} /LTCG")
    set(CMAKE_SHARED_LINKER_FLAGS_${_config}  "${CMAKE_SHARED_LINKER_FLAGS_${_config}} /LTCG")
    set(CMAKE_STATIC_LINKER_FLAGS_${_config}  "${CMAKE_STATIC_LINKER_FLAGS_${_config}} /LTCG")
endforeach ()

project(AntijankProfiler)

set(CMAKE_CXX_STANDARD 20)

add_library(AntijankProfiler SHARED
        AntijankProfiler.cpp AntijankProfiler.h AntijankProfiler.def
        AntijankProfilerClassFactory.cpp AntijankProfilerClassFactory.h
        Plumbing.cpp Plumbing.h pair_hash.h)

set_property(TARGET AntijankProfiler PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

target_include_directories(AntijankProfiler PUBLIC
        "$ENV{ProgramFiles\(x86\)}\\Windows Kits\\NETFXSDK\\4.8\\Include\\um")