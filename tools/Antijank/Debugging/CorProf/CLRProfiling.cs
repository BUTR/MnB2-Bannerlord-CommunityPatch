using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("B349ABE3-B56F-4689-BFCD-76BF39D888EA")]
  [CoClass(typeof(CorDebugClass))]
  [ComImport]
  
  public interface CLRProfiling : ICLRProfiling {

  }

}