using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Guid("D28F3C5A-9634-4206-A509-477552EEFB10")]
  [CoClass(typeof(CLRDebuggingClass))]
  [ComImport]
  
  public interface CLRDebugging : ICLRDebugging {

  }

}