using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Sequential)]
  [PublicAPI]
  public struct COR_GC_REFERENCE {

    public ICorDebugAppDomain Domain;

    public ICorDebugValue Location;

    public CorGCReferenceType Type;

    public ulong ExtraData;

  }

}