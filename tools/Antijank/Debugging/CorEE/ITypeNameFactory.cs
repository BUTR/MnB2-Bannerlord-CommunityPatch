using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("B81FF171-20F3-11D2-8DCC-00A0C9B00521")]
  [ComImport]
  
  public interface ITypeNameFactory {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ITypeName ParseTypeName([MarshalAs(UnmanagedType.LPWStr)] [In] string szName, out uint pError);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    ITypeNameBuilder GetTypeNameBuilder();

  }

}