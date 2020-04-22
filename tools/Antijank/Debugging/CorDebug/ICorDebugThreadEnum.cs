using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("CC7BCB06-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugThreadEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugThreadEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugThreadEnum threads, out uint pceltFetched);

  }

}