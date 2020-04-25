using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [StructLayout(LayoutKind.Sequential)]
  [PublicAPI]
  public struct COR_TYPE_LAYOUT {

    public COR_TYPEID parentID;

    public int objectSize;

    public int numFields;

    public int boxOffset;

    public CorElementType type;

  }

}