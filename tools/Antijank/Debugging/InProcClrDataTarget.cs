using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Antijank.Interop;

namespace Antijank.Debugging {

  
  public class InProcClrDataTarget : ICLRDataTarget3, ICLRMetadataLocator, IDisposable {

    private GCHandle _keepAlive;

    public InProcClrDataTarget()
      => _keepAlive = GCHandle.Alloc(this, GCHandleType.Normal);

    public void GetMachineType(out IMAGE_FILE_MACHINE machineType) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetMachineType)}");
      Console.Error.Flush();

      switch (RuntimeInformation.ProcessArchitecture) {
        case Architecture.X86:
          machineType = IMAGE_FILE_MACHINE.I386;
          break;
        case Architecture.X64:
          machineType = IMAGE_FILE_MACHINE.AMD64;
          break;
        case Architecture.Arm:
          machineType = IMAGE_FILE_MACHINE.ARM;
          break;
        case Architecture.Arm64:
          machineType = IMAGE_FILE_MACHINE.ARM64;
          break;
        default:
          throw new PlatformNotSupportedException(RuntimeInformation.ProcessArchitecture.ToString());
      }
    }

    public void GetPointerSize(out uint pointerSize) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetPointerSize)}");
      Console.Error.Flush();
      pointerSize = unchecked((uint) IntPtr.Size);
    }

    public void GetImageBase(string imagePath, out ulong baseAddress) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetImageBase)}");
      Console.Error.Flush();
      using (var proc = Process.GetCurrentProcess()) {
        foreach (ProcessModule mod in proc.Modules) {
          if (imagePath == mod.FileName) {
            baseAddress = unchecked((ulong) mod.BaseAddress.ToInt64());
            return;
          }

          var modFileName = Path.GetFileName(mod.FileName);
          if (imagePath == modFileName) {
            baseAddress = unchecked((ulong) mod.BaseAddress.ToInt64());
            return;
          }

          var imgFileName = Path.GetFileName(imagePath);
          if (imgFileName == modFileName) {
            baseAddress = unchecked((ulong) mod.BaseAddress.ToInt64());
            return;
          }
        }
      }

      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public unsafe void ReadVirtual(ulong address, ref byte* buffer, uint bytesRequested, out uint bytesRead) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(ReadVirtual)}");
      Console.Error.Flush();
      Unsafe.CopyBlockUnaligned(buffer, (byte*) address, bytesRequested);
      bytesRead = bytesRequested;
    }

    public unsafe void WriteVirtual(ulong address, ref byte* buffer, uint bytesRequested, out uint bytesWritten) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(WriteVirtual)}");
      Console.Error.Flush();
      Unsafe.CopyBlockUnaligned((byte*) address, buffer, bytesRequested);
      bytesWritten = bytesRequested;
    }

    public void GetTLSValue(uint threadID, uint index, out ulong value) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetTLSValue)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public void SetTLSValue(uint threadID, uint index, ulong value) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(SetTLSValue)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public void GetCurrentThreadID(out uint threadID) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetCurrentThreadID)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public unsafe void GetThreadContext(uint threadID, uint contextFlags, uint contextSize, ref byte* context) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetThreadContext)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public unsafe void SetThreadContext(uint threadID, uint contextSize, ref byte* context) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(SetThreadContext)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public unsafe void Request(uint reqCode, uint inBufferSize, ref byte* inBuffer, uint outBufferSize,
      ref byte* outBuffer) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(Request)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public void AllocVirtual(ulong addr, uint size, uint typeFlags, uint protectFlags, out ulong virt) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(AllocVirtual)}");
      Console.Error.Flush();
      throw new NotImplementedException();
    }

    public void FreeVirtual(ulong addr, uint size, uint typeFlags) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(FreeVirtual)}");
      Console.Error.Flush();
      throw new NotImplementedException();
    }

    public unsafe void GetExceptionRecord(uint bufferSize, out uint bufferUsed, ref byte* buffer) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetExceptionRecord)}");
      Console.Error.Flush();
      throw new NotImplementedException();
    }

    public unsafe void GetExceptionContextRecord(uint bufferSize, out uint bufferUsed, ref byte* buffer) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetExceptionContextRecord)}");
      Console.Error.Flush();
      throw new NotImplementedException();
    }

    public void GetExceptionThreadID(out uint threadID) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetExceptionThreadID)}");
      Console.Error.Flush();
      throw new NotImplementedException();
    }

    public unsafe void GetMetadata(string imagePath, uint imageTimestamp, uint imageSize,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid mvId, uint mdRva,
      uint flags, uint bufferSize, ref byte* buffer, out uint dataSize) {
      Console.Error.WriteLine($"{nameof(InProcClrDataTarget)}.{nameof(GetMetadata)}");
      Console.Error.Flush();
      throw Marshal.GetExceptionForHR(unchecked((int) 0x80004005)); // E_FAIL
    }

    public void Dispose()
      => _keepAlive.Free();

  }

}