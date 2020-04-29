using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  [Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
  [CoClass(typeof(CLRMetaHostClass))]
  [ComImport]
  
  public interface CLRMetaHost : ICLRMetaHost {

  }

}