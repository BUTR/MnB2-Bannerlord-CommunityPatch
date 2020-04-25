using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("D613F0BB-ACE1-4C19-BD72-E4C08D5DA7F5")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  [PublicAPI]
  public interface ICorDebugType {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("ty")]
    uint GetType();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppClass")]
    ICorDebugClass GetClass();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppTyParEnum")]
    ICorDebugTypeEnum EnumerateTypeParameters();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("value")]
    ICorDebugType GetFirstTypeParameter();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("pBase")]
    ICorDebugType GetBase();

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: MarshalAs(UnmanagedType.Interface)]
    [return: Description("ppValue")]
    ICorDebugValue GetStaticFieldValue([In] uint fieldDef, [MarshalAs(UnmanagedType.Interface)] [In]
      ICorDebugFrame pFrame);

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: Description("pnRank")]
    uint GetRank();

  }

}