using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("3721A26F-8B91-4D98-A388-DB17B356FADB")]
  [ComImport]
  
  public interface ICLRDataEnumMemoryRegionsCallback2 : ICLRDataEnumMemoryRegionsCallback {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumMemoryRegion([In] ulong address, [In] uint size);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void UpdateMemoryRegion([In] ulong address, [In] uint bufferSize, [In] ref byte* buffer);

  }

}