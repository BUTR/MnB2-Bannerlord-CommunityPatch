using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("CC7BCAF9-8A68-11D2-983C-0000F808342D")]
  [ComImport]
  
  public interface ICorDebugReferenceValue : ICorDebugValue {

    /*
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetType(out uint pType);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out uint pSize);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong pAddress);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsNull(out int pbNull);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetValue(out ulong pValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetValue([In] ulong value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Dereference([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void DereferenceStrong([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

  }

}