using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("DBA2D8C1-E5C5-4069-8C13-10A7C6ABF43D")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugModule {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppProcess")]
    ICorDebugProcess GetProcess();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pAddress")]
    ulong GetBaseAddress();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppAssembly")]
    ICorDebugAssembly GetAssembly();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 0)] [Out]
      StringBuilder szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableJITDebugging(
      [MarshalAs(UnmanagedType.Bool)] [In] bool bTrackJITInfo,
      [MarshalAs(UnmanagedType.Bool)] [In] bool bAllowJitOpts);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void EnableClassLoadCallbacks(
      [MarshalAs(UnmanagedType.Bool)] [In] bool bClassLoadCallbacks);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppFunction")]
    ICorDebugFunction GetFunctionFromToken([In] uint methodDef);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppFunction")]
    ICorDebugFunction GetFunctionFromRVA([In] ulong rva);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppClass")]
    ICorDebugClass GetClassFromToken([In] uint typeDef);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppBreakpoint")]
    ICorDebugModuleBreakpoint CreateBreakpoint();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppEditAndContinueSnapshot")]
    ICorDebugEditAndContinueSnapshot GetEditAndContinueSnapshot();

    // TODO: translate IMetaDataImport, IMetaDataImport2 from cor.h
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.IUnknown), Description("ppObj")]
    object GetMetaDataInterface(
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid rIid);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pToken")]
    uint GetToken();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Bool), Description("pDynamic")]
    bool IsDynamic();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppValue")]
    ICorDebugValue GetGlobalVariableValue([In] uint fieldDef);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pcBytes")]
    void GetSize();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Bool), Description("pInMemory")]
    bool IsInMemory();

  }

}