using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("D28F3C5A-9634-4206-A509-477552EEFB10")]
  [CoClass(typeof(CLRDebuggingClass))]
  [ComImport]
  [PublicAPI]
  public interface CLRDebugging : ICLRDebugging { }

}