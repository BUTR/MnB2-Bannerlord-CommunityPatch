using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [CoClass(typeof(CorMetaDataDispenserExClass))]
  [Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
  [ComImport]
  
  public interface CorMetaDataDispenserEx : IMetaDataDispenserEx {

  }

}