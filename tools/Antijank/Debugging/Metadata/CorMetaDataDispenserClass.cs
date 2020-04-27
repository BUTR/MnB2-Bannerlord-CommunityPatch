using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("E5CB7A31-7512-11D2-89CE-0080C792E5D8")]
  [ComImport]
  
  public class CorMetaDataDispenserClass : CorMetaDataDispenser {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint DefineScope(ref Guid rclsid, uint dwCreateFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint OpenScope([MarshalAs(UnmanagedType.LPWStr)] string szScope, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint OpenScopeOnMemory(IntPtr pData, uint cbData, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

  }

}