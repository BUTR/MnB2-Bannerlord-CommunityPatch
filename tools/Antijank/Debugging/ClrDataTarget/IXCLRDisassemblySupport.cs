using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("1F0F7134-D3F3-47DE-8E9B-C2FD358A2936")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface IXCLRDisassemblySupport {

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020005(
      [MarshalAs(UnmanagedType.Interface)] IXCLRDisassemblySupport __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020000,
      ulong __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020001, ref ushort __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020002,
      UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020003,
      ref ulong __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020004);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTranslateAddrCallback([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDisassemblySupport cb);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void PvClientSet([In] IntPtr pv);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr CbDisassemble(ulong __MIDL__IXCLRDisassemblySupport0000, IntPtr __MIDL__IXCLRDisassemblySupport0001,
      UIntPtr __MIDL__IXCLRDisassemblySupport0002);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr Cinstruction();

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    int FSelectInstruction(UIntPtr __MIDL__IXCLRDisassemblySupport0003);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr CchFormatInstr(ref ushort __MIDL__IXCLRDisassemblySupport0004, UIntPtr __MIDL__IXCLRDisassemblySupport0005);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    IntPtr PvClient();

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020012(
      [MarshalAs(UnmanagedType.Interface)] IXCLRDisassemblySupport __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020006,
      ulong __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020007, UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020008,
      ref ushort __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020009,
      UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020010,
      ref ulong __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020011);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTranslateFixupCallback([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDisassemblySupport cb);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020017(
      [MarshalAs(UnmanagedType.Interface)] IXCLRDisassemblySupport __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020013,
      uint __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020014, ref ushort __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020015,
      UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020016);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTranslateConstCallback([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDisassemblySupport cb);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020023(
      [MarshalAs(UnmanagedType.Interface)] IXCLRDisassemblySupport __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020018,
      uint rega, ulong __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020019,
      ref ushort __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020020,
      UIntPtr __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020021,
      ref uint __MIDL____MIDL_itf_tylib2Dclrdata_0000_00020022);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void SetTranslateRegrelCallback([MarshalAs(UnmanagedType.Interface)] [In]
      IXCLRDisassemblySupport cb);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
    int TargetIsAddress();

  }

}