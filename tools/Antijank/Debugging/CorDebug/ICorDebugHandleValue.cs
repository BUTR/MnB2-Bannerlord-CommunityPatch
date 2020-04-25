using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("029596E8-276B-46A1-9821-732E96BBB00B")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugHandleValue : ICorDebugReferenceValue {

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
    void IsNull(out int pbNull);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetValue(out ulong pValue);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetValue([In] ulong value);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void Dereference([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    [MethodImpl(MethodImplOptions.InternalCall)]
    void DereferenceStrong([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    */
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pType")]
    CorDebugHandleType GetHandleType();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Dispose();

  }

}