using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Sequential)]
  
  public struct COR_GC_REFERENCE {

    public ICorDebugAppDomain Domain;

    public ICorDebugValue Location;

    public CorGCReferenceType Type;

    public ulong ExtraData;

  }

}