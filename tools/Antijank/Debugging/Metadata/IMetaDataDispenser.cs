using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  [ComImport, Guid("809C652E-7396-11D2-9771-00A0C9B4D50C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataDispenser {

    uint DefineScope(
      ref Guid rclsid,
      uint dwCreateFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
      out object ppIUnk
    );

    uint OpenScope(
      [MarshalAs(UnmanagedType.LPWStr)] string szScope,
      uint dwOpenFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)]
      out object ppIUnk
    );

    uint OpenScopeOnMemory(
      IntPtr pData,
      uint cbData,
      uint dwOpenFlags,
      ref Guid riid,
      [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 3)]
      out object ppIUnk
    );

  }

}