using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Antijank.Debugging {

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("46586093-D3F5-4DB6-ACDB-955BCE228C15")]
  [SuppressUnmanagedCodeSecurity]
  [ComImport]
  public interface ICorDebugILCode2 {

    [MethodImpl(MethodImplOptions.InternalCall)]
    [return: ComAliasName("pmdSig")]
    uint GetLocalVarSigToken();

    [MethodImpl(MethodImplOptions.InternalCall)]
    void GetInstrumentedILMap(
      [In] uint cMap,
      [Out] out uint pcMap,
      [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      COR_IL_MAP[] map);

  }

}