using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("FF71301A-B994-429D-A10B-B345A65280EF")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorProfilerFunctionEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Clone([MarshalAs(UnmanagedType.Interface)] out ICorProfilerFunctionEnum ppEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerFunctionEnum ids, out uint pceltFetched);

  }

}