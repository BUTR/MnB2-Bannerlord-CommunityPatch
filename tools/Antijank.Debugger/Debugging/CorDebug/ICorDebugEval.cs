using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [Guid("CC7BCAF6-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugEval {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CallFunction([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFunction pFunction, [In] uint nArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugValue ppArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewObject([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFunction pConstructor, [In] uint nArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugValue ppArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewObjectNoConstructor([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass pClass);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewString([MarshalAs(UnmanagedType.LPWStr)] [In] string @string);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewArray([In] uint elementType, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass pElementClass, [In] uint rank, [In] ref uint dims, [In] ref uint lowBounds);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsActive(out int pbActive);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Abort();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetResult([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppResult);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetThread([MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateValue([In] uint elementType, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass pElementClass, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

  }

}