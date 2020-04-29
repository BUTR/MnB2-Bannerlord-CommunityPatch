using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [Guid("B0266D75-2081-4493-AF7F-028BA34DB891")]
  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICorProfilerModuleEnum {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Reset();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Clone([MarshalAs(UnmanagedType.Interface)] out ICorProfilerModuleEnum ppEnum);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcelt")]
    uint GetCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Next(
      [In] uint celt,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out]
      UIntPtr[] ids,
      out uint pceltFetched
    );

  }

}