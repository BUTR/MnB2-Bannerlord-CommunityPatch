using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("E2190695-77B2-492E-8E14-C4B3A7FDD593")]
  [CoClass(typeof(CLRMetaHostPolicyClass))]
  [ComImport]
  
  public interface CLRMetaHostPolicy : ICLRMetaHostPolicy {

  }

}