using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.Diagnostics.Runtime;
using Architecture = Microsoft.Diagnostics.Runtime.Architecture;

namespace Antijank {

  
  public class InProcDataReader : IDataReader {

    private IDataReader _liveDataReader;

    public InProcDataReader(IDataReader liveDataReader)
      => _liveDataReader = liveDataReader;

    public void Dispose() {
      _liveDataReader?.Dispose();
      _liveDataReader = null;
    }

    public unsafe bool Read(ulong address, Span<byte> buffer, out int bytesRead) {
      try {
        fixed (void* pBuf = buffer)
          Unsafe.CopyBlockUnaligned(pBuf, (void*) address, (uint) buffer.Length);
        bytesRead = buffer.Length;
        return true;
      }
      catch {
        bytesRead = 0;
        return false;
      }
    }

    public unsafe bool Read<T>(ulong address, out T value) where T : unmanaged {
      try {
        value = Unsafe.ReadUnaligned<T>((void*) address);
        return true;
      }
      catch {
        value = default;
        return false;
      }
    }

    public unsafe T Read<T>(ulong address) where T : unmanaged
      => Unsafe.ReadUnaligned<T>((void*) address);

    public unsafe bool ReadPointer(ulong address, out ulong value) {
      try {
        value = Unsafe.ReadUnaligned<UIntPtr>((void*) address).ToUInt64();
        return true;
      }
      catch {
        value = default;
        return false;
      }
    }

    public unsafe ulong ReadPointer(ulong address)
      => Unsafe.ReadUnaligned<UIntPtr>((void*) address).ToUInt64();

    public unsafe int PointerSize
      => sizeof(void*);

    public IEnumerable<uint> EnumerateAllThreads()
      => _liveDataReader.EnumerateAllThreads();

    public IEnumerable<ModuleInfo> EnumerateModules()
      => _liveDataReader.EnumerateModules();

    public void GetVersionInfo(ulong baseAddress, out VersionInfo version)
      => _liveDataReader.GetVersionInfo(baseAddress, out version);

    public bool QueryMemory(ulong address, out MemoryRegionInfo info)
      => _liveDataReader.QueryMemory(address, out info);

    public bool GetThreadContext(uint threadId, uint contextFlags, Span<byte> context)
      => _liveDataReader.GetThreadContext(threadId, contextFlags, context);

    public void FlushCachedData()
      => _liveDataReader.FlushCachedData();

    public bool IsThreadSafe => true;

    public Architecture Architecture => RuntimeInformation.ProcessArchitecture switch {
      System.Runtime.InteropServices.Architecture.X86 => Architecture.X86,
      System.Runtime.InteropServices.Architecture.X64 => Architecture.Amd64,
      System.Runtime.InteropServices.Architecture.Arm => Architecture.Arm,
      System.Runtime.InteropServices.Architecture.Arm64 => Architecture.Arm64,
      _ => throw new PlatformNotSupportedException()
    };

    public uint ProcessId => _liveDataReader.ProcessId;

    public bool IsFullMemoryAvailable => true;

  }

}