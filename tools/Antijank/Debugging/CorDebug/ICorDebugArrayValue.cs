using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("0405B0DF-A660-11D2-BD02-0000F80849BD")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugArrayValue : ICorDebugHeapValue {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetType(out uint pType);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out uint pSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong pAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsValid(out int pbValid);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateRelocBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetElementType(out uint pType);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetRank(out uint pnRank);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCount(out uint pnCount);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetDimensions([In] uint cdim, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugArrayValue dims);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void HasBaseIndicies(out int pbHasBaseIndicies);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetBaseIndicies([In] uint cdim, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugArrayValue indicies);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetElement([In] uint cdim, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugArrayValue indices, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetElementAtPosition([In] uint nPosition, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

  }

}