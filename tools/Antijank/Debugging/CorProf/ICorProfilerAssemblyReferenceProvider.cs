using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("66A78C24-2EEF-4F65-B45F-DD1D8038BF3C")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorProfilerAssemblyReferenceProvider {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddAssemblyReference(ref COR_PRF_ASSEMBLY_REFERENCE_INFO pAssemblyRefInfo);

  }

}