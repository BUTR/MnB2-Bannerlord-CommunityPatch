using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace Antijank.Debugging {

  
  [ComImport, Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataDispenserEx : IMetaDataDispenser {

    new uint DefineScope(
      ref Guid rclsid,
      uint dwCreateFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
      out object ppIUnk
    );

    new uint OpenScope(
      [MarshalAs(UnmanagedType.LPWStr)] string szScope,
      uint dwOpenFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
      out object ppIUnk
    );

    new uint OpenScopeOnMemory(
      IntPtr pData,
      uint cbData,
      uint dwOpenFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 3)]
      out object ppIUnk
    );

    void SetOption([In] in Guid optionId, [In, MarshalAs(UnmanagedType.Struct)] in object pValue);

    [return: MarshalAs(UnmanagedType.Struct)]
    object GetOption([In] in Guid optionId);

    uint OpenScopeOnITypeInfo(
      [MarshalAs(UnmanagedType.Interface)] ITypeInfo pITI,
      uint dwOpenFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
      out object ppIUnk
    );

    uint GetCORSystemDirectory(
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
      char[] szBuffer, uint cchBuffer, out uint pchBuffer);

    uint FindAssembly(
      [MarshalAs(UnmanagedType.LPWStr)] string szAppBase,
      [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
      char[] szName, uint cchName, out uint pcName);

    uint FindAssemblyModule(
      [MarshalAs(UnmanagedType.LPWStr)] string szAppBase,
      [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin,
      [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName,
      [MarshalAs(UnmanagedType.LPWStr)] string szModuleName,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)]
      char[] szName, uint cchName, out uint pcName);

  }

}