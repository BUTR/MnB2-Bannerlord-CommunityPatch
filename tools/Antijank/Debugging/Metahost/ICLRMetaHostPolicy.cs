using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("E2190695-77B2-492E-8E14-C4B3A7FDD593")]
  [ComImport]
  [PublicAPI]
  public interface ICLRMetaHostPolicy {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface, IidParameterIndex = 8)]
    object GetRequestedRuntime(
      [In] METAHOST_POLICY_FLAGS dwPolicyFlags,
      [MarshalAs(UnmanagedType.LPWStr)] [In] string pwzBinary,
      [MarshalAs(UnmanagedType.Interface)] [In]
      IStream pCfgStream,
      [MarshalAs(UnmanagedType.LPWStr)] [In] [Out]
      StringBuilder pwzVersion,
      [In] [Out] ref uint pcchVersion,
      [MarshalAs(UnmanagedType.LPWStr)] [Out]
      StringBuilder pwzImageVersion,
      [In] [Out] ref uint pcchImageVersion,
      out uint pdwConfigFlags,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid
    );

  }

}