using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("571194F7-25ED-419F-AA8B-7016B3159701")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorProfilerThreadEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Clone([MarshalAs(UnmanagedType.Interface)] out ICorProfilerThreadEnum ppEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorProfilerThreadEnum ids, out uint pceltFetched);

  }

}