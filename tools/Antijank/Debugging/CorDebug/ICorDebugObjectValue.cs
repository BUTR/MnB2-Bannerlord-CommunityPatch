using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("18AD3D6E-B7D2-11D2-BD04-0000F80849BD")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugObjectValue : ICorDebugValue {

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
    void GetClass([MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFieldValue([MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugClass pClass, [In] uint fieldDef, [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVirtualMethod([In] uint memberRef, [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetContext([MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsValueClass(out int pbIsValueClass);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetManagedCopy([MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetFromManagedCopy([MarshalAs(UnmanagedType.IUnknown)] [In]
      object pObject);

  }

}