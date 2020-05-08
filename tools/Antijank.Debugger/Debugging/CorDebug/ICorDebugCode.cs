using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("CC7BCAF4-8A68-11D2-983C-0000F808342D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICorDebugCode {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Bool), ComAliasName("pbIL")]
    bool IsIL();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppFunction")]
    ICorDebugFunction GetFunction();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pStart")]
    IntPtr GetAddress();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pcBytes")]
    uint GetSize();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("ppBreakpoint")]
    ICorDebugFunctionBreakpoint CreateBreakpoint([In] uint offset);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCode([In] uint startOffset, [In] uint endOffset, [In] uint cBufferAlloc,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugCode buffer, out uint pcBufferSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("nVersion")]
    uint GetVersionNumber();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("map")]
    ICorDebugCode GetILToNativeMapping([In] uint cMap, out uint pcMap);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), ComAliasName("offsets")]
    ICorDebugCode GetEnCRemapSequencePoints([In] uint cMap, out uint pcMap);

  }

}