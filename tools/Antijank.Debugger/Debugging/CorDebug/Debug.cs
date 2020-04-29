using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Antijank.Debugging {

  public static class Debug {

    private const string DbgShim = "dbgshim"; //"mscoree";

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern unsafe int EnumerateCLRs(
      uint debuggeePID,
      //[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt, SizeParamIndex= 3)]
      [Out] out UIntPtr ppHandleArrayOut,
      //[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 3)]
      [Out] out char** ppStringArrayOut,
      [Out] out uint pdwArrayLengthOut);

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern unsafe int CloseCLREnumeration(
      //[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt, SizeParamIndex= 3)]
      [In] UIntPtr ppHandleArrayOut,
      //[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 3)]
      [In] char** ppStringArrayOut,
      [In] uint pdwArrayLengthOut
    );

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern int CreateVersionStringFromModule(
      [In] uint pidDebuggee,
      [In] string szModuleName,
      [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 3)] [Out]
      StringBuilder pBuffer,
      [In] uint cchBuffer,
      [Out] out uint pdwLength);

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern int CreateDebuggingInterfaceFromVersion(
      [MarshalAs(UnmanagedType.LPWStr)] string szDebuggeeVersion,
      [Out] [MarshalAs(UnmanagedType.Interface)]
      out object pCorDb);

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern int CreateDebuggingInterfaceFromVersionEx(
      [MarshalAs(UnmanagedType.I4)] CorDebugInterfaceVersion iDebuggerVersion,
      [MarshalAs(UnmanagedType.LPWStr)] string szDebuggeeVersion,
      [Out] [MarshalAs(UnmanagedType.Interface)]
      out object pCorDb);

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern int CreateDebuggingInterfaceFromVersion2(
      [MarshalAs(UnmanagedType.I4)] CorDebugInterfaceVersion iDebuggerVersion,
      [MarshalAs(UnmanagedType.LPWStr)] string szDebuggeeVersion,
      [MarshalAs(UnmanagedType.LPWStr)] string szApplicationGroupId,
      [Out] [MarshalAs(UnmanagedType.Interface)]
      out object pCorDb);

    [DllImport(DbgShim, CharSet = CharSet.Unicode)]
    public static extern int GetRequestedRuntimeVersion(
      string pExe,
      StringBuilder pVersion,
      int cchBuffer,
      out int dwLength);

    [DllImport(DbgShim, CharSet = CharSet.Unicode, PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public static extern object CLRCreateInstance(
      [MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
      [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

  }

}