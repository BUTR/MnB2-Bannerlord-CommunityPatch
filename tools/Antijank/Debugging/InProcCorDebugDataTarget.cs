using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public class InProcCorDebugDataTarget : ICorDebugMutableDataTarget, ICorDebugMetaDataLocator {

    private GCHandle _keepAlive;

    public InProcCorDebugDataTarget()
      => _keepAlive = GCHandle.Alloc(this, GCHandleType.Normal);

    public void GetMetaData(string wszImagePath, uint dwImageTimeStamp, uint dwImageSize, uint cchPathBuffer,
      out uint pCchPathBuffer, ICorDebugMetaDataLocator wszPathBuffer) {
      Console.Error.WriteLine($"{nameof(InProcCorDebugDataTarget)}.{nameof(GetMetaData)}");
      Console.Error.Flush();
      System.Diagnostics.Debugger.Break();
      throw new NotImplementedException();
    }

    public void GetPlatform(out CorDebugPlatform pTargetPlatform) {
      Console.Error.WriteLine($"{nameof(InProcCorDebugDataTarget)}.{nameof(GetPlatform)}");
      Console.Error.Flush();
      switch (RuntimeInformation.ProcessArchitecture) {
        case Architecture.X86:
          if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_WINDOWS_X86;
          else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_MAC_X86;
          else
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_POSIX_X86;
          break;
        case Architecture.X64:
          if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_WINDOWS_AMD64;
          else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_MAC_AMD64;
          else
            pTargetPlatform = CorDebugPlatform.CORDB_PLATFORM_POSIX_AMD64;
          break;
        case Architecture.Arm:
          pTargetPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? CorDebugPlatform.CORDB_PLATFORM_WINDOWS_ARM
            : CorDebugPlatform.CORDB_PLATFORM_POSIX_ARM;
          break;
        case Architecture.Arm64:
          pTargetPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? CorDebugPlatform.CORDB_PLATFORM_WINDOWS_ARM64
            : CorDebugPlatform.CORDB_PLATFORM_POSIX_ARM64;
          break;
        default:
          throw new PlatformNotSupportedException(RuntimeInformation.ProcessArchitecture.ToString());
      }
    }

    public unsafe void ReadVirtual(ulong address, byte* pBuffer, uint bytesRequested, out uint pBytesRead) {
      Unsafe.CopyBlockUnaligned(pBuffer, (void*) address, bytesRequested);
      pBytesRead = bytesRequested;
    }

    public unsafe void WriteVirtual(ulong address, byte* pBuffer, uint bytesRequested)
      => Unsafe.CopyBlockUnaligned((void*) address, pBuffer, bytesRequested);

    public unsafe void GetThreadContext(uint dwThreadId, uint contextFlags, uint contextSize, byte* pContext) {
      Console.Error.WriteLine($"{nameof(InProcCorDebugDataTarget)}.{nameof(GetThreadContext)}");
      Console.Error.Flush();
      //throw new NotImplementedException();

      // https://github.com/dotnet/coreclr/blob/master/src/pal/src/thread/context.cpp#L273-L287
      Unsafe.InitBlockUnaligned(pContext, 0, contextSize);
    }

    public unsafe void SetThreadContext(uint dwThreadId, uint contextSize, byte* pContext) {
      Console.Error.WriteLine($"{nameof(InProcCorDebugDataTarget)}.{nameof(SetThreadContext)}");
      Console.Error.Flush();
      System.Diagnostics.Debugger.Break();
      throw new NotImplementedException();
    }

    public void ContinueStatusChanged(uint dwThreadId, uint continueStatus) {
      Console.Error.WriteLine($"{nameof(InProcCorDebugDataTarget)}.{nameof(ContinueStatusChanged)}");
      Console.Error.Flush();
      System.Diagnostics.Debugger.Break();
      throw new NotImplementedException();
    }

    public void Dispose()
      => _keepAlive.Free();

  }

}