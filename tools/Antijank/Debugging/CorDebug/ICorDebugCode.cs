using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("CC7BCAF4-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugCode {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void IsIL(out int pbIL);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetAddress(out ulong pStart);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSize(out uint pcBytes);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CreateBreakpoint([In] uint offset,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCode([In] uint startOffset, [In] uint endOffset, [In] uint cBufferAlloc,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugCode buffer, out uint pcBufferSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetVersionNumber(out uint nVersion);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetILToNativeMapping([In] uint cMap, out uint pcMap, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugCode map);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetEnCRemapSequencePoints([In] uint cMap, out uint pcMap, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugCode offsets);

  }

}