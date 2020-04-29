using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CC7BCB05-8A68-11D2-983C-0000F808342D")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugProcessEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ICorDebugProcessEnum Clone();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next([In] uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface)] [Out]
      ICorDebugProcess[] processes, out uint pceltFetched);

  }

}