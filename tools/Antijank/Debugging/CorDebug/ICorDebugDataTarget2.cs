using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [Guid("2EB364DA-605B-4E8D-B333-3394C4828D41")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  
  public interface ICorDebugDataTarget2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetImageFromPointer([In] ulong addr, out ulong pImageBase, out uint pSize);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetImageLocation([In] ulong baseAddress, [In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugDataTarget2 szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetSymbolProviderForImage([In] ulong imageBaseAddress,
      [MarshalAs(UnmanagedType.Interface)] out ICorDebugSymbolProvider ppSymProvider);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnumerateThreadIDs([In] uint cThreadIds, out uint pcThreadIds, [MarshalAs(UnmanagedType.Interface)] [Out]
      ICorDebugDataTarget2 pThreadIds);

    [MethodImpl(MethodImplOptions.InternalCall)]
    unsafe void CreateVirtualUnwinder([In] uint nativeThreadID, [In] uint contextFlags, [In] uint cbContext,
      [In] ref byte* initialContext, [MarshalAs(UnmanagedType.Interface)] out ICorDebugVirtualUnwinder ppUnwinder);

  }

}