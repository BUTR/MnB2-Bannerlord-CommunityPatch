using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("FB0D9CE7-BE66-4683-9D32-A42A04E2FD91")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugEval2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CallParameterizedFunction([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFunction pFunction, [In] uint nTypeArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugType ppTypeArgs, [In] uint nArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugValue ppArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateValueForType([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugType pType, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewParameterizedObject([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFunction pConstructor, [In] uint nTypeArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugType ppTypeArgs, [In] uint nArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugValue ppArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewParameterizedObjectNoConstructor([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass pClass, [In] uint nTypeArgs, [MarshalAs(UnmanagedType.Interface)] [In]
      ref ICorDebugType ppTypeArgs);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewParameterizedArray([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugType pElementType, [In] uint rank, [In] ref uint dims, [In] ref uint lowBounds);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void NewStringWithLength([MarshalAs(UnmanagedType.LPWStr)] [In] string @string, [In] uint uiLength);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void RudeAbort();

  }

}