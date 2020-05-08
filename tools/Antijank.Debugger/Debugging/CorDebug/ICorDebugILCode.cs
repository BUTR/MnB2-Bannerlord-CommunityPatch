using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("598D46C2-C877-42A7-89D2-3D0C7F1C1264")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorDebugILCode {

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetEHClauses(
      [In] uint cClauses,
      [Out] out uint pcClauses,
      [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      CorDebugEHClause[] clauses);

  }

}