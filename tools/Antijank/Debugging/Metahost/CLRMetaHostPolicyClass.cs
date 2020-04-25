using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [Guid("2ebcd49a-1b47-4a61-b13a-4a03701e594b")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public class CLRMetaHostPolicyClass : ICLRMetaHostPolicy {

    [MethodImpl(MethodImplOptions.InternalCall)]
    public virtual extern object GetRequestedRuntime(
      METAHOST_POLICY_FLAGS dwPolicyFlags,
      string pwzBinary,
      IStream pCfgStream,
      StringBuilder pwzVersion,
      ref uint pcchVersion,
      StringBuilder pwzImageVersion,
      ref uint pcchImageVersion,
      out uint pdwConfigFlags,
      [In] [MarshalAs(UnmanagedType.LPStruct)]
      Guid riid
    );

  }

}