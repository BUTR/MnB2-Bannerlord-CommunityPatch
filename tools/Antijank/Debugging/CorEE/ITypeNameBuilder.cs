using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [SuppressUnmanagedCodeSecurity]
  [Guid("B81FF171-20F3-11D2-8DCC-00A0C9B00523")]
  [ComImport]
  
  public interface ITypeNameBuilder {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OpenGenericArguments();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CloseGenericArguments();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void OpenGenericArgument();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void CloseGenericArgument();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddName([MarshalAs(UnmanagedType.LPWStr)] [In] string szName);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddPointer();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddByRef();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddSzArray();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddArray([In] uint rank);

    [MethodImpl(MethodImplOptions.InternalCall)]
    void AddAssemblySpec([MarshalAs(UnmanagedType.LPWStr)] [In] string szAssemblySpec);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string ToString();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void Clear();

  }

}