using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [ClassInterface(ClassInterfaceType.None)]
  [Guid("B81FF171-20F3-11D2-8DCC-00A0C9B00525")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  
  public class TypeNameFactoryClass : ITypeNameFactory, TypeNameFactory {

    //[MethodImpl(MethodImplOptions.InternalCall)]
    //public extern TypeNameFactoryClass();
    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern ITypeName
      ParseTypeName([MarshalAs(UnmanagedType.LPWStr)] [In] string szName, out uint pError);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern ITypeNameBuilder GetTypeNameBuilder();

  }

}