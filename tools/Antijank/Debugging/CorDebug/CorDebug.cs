using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF")]
  [CoClass(typeof(CorDebugClass))]
  [ComImport]
  [PublicAPI]
  public interface CorDebug : ICorDebug {

  }

}