using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
  [CoClass(typeof(CLRMetaHostClass))]
  [ComImport]
  [PublicAPI]
  public interface CLRMetaHost : ICLRMetaHost {

  }

}