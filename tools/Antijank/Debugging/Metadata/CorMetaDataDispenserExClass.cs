using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using Antijank.Interop;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("E5CB7A31-7512-11D2-89CE-0080C792E5D8")]
  [ComImport]
  [PublicAPI]
  public class CorMetaDataDispenserExClass : CorMetaDataDispenserEx {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint DefineScope(ref Guid rclsid, uint dwCreateFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint OpenScope([MarshalAs(UnmanagedType.LPWStr)] string szScope, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint OpenScopeOnMemory(IntPtr pData, uint cbData, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern void SetOption([In] in Guid optionId, [In, MarshalAs(UnmanagedType.Struct)] in object pValue);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Struct)]
    public virtual extern object GetOption([In] in Guid optionId);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint OpenScopeOnITypeInfo([MarshalAs(UnmanagedType.Interface)] ITypeInfo pITI, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint GetCORSystemDirectory([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      char[] szBuffer, uint cchBuffer, out uint pchBuffer);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint FindAssembly([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      char[] szName, uint cchName, out uint pcName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern uint FindAssemblyModule([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, [MarshalAs(UnmanagedType.LPWStr)] string szModuleName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      char[] szName, uint cchName, out uint pcName);

  }

}