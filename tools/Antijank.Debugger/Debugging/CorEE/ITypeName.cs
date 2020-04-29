using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


namespace Antijank.Debugging {

  [SuppressUnmanagedCodeSecurity]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("B81FF171-20F3-11D2-8DCC-00A0C9B00522")]
  [ComImport]
  
  public interface ITypeName {

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetNameCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetNames([In] uint count, [MarshalAs(UnmanagedType.BStr)] out string rgbszNames);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetTypeArgumentCount();

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetTypeArguments([In] uint count, [MarshalAs(UnmanagedType.Interface)] out ITypeName rgpArguments);

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetModifierLength();

    [MethodImpl(MethodImplOptions.InternalCall)]
    uint GetModifiers([In] uint count, out uint rgModifiers);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetAssemblyName();

  }

}