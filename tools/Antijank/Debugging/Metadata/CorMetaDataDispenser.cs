using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [CoClass(typeof(CorMetaDataDispenserClass))]
  [Guid("809C652E-7396-11D2-9771-00A0C9B4D50C")]
  [ComImport]
  
  public interface CorMetaDataDispenser : IMetaDataDispenser {

  }

}