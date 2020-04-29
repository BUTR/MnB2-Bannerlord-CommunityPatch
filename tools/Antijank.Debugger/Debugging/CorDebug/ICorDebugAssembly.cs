using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;


namespace Antijank.Debugging {

  [Guid("DF59507C-D47A-459E-BCE2-6427EAC8FD06")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public interface ICorDebugAssembly {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppProcess")]
    ICorDebugProcess GetProcess();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppAppDomain")]
    ICorDebugAppDomain GetAppDomain();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface), Description("ppModules")]
    ICorDebugModuleEnum EnumerateModules();

    //[Obsolete("NOT IMPLEMENTED", true)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetCodeBase([In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 0)] [Out]
      StringBuilder szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetName([In] uint cchName, out uint pcchName,
      [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 0)] [Out]
      StringBuilder szName);

  }

}