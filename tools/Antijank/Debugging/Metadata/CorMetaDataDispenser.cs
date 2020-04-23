using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [CoClass(typeof(CorMetaDataDispenserClass))]
  [Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
  [ComImport]
  [PublicAPI]
  public interface CorMetaDataDispenser : IMetaDataDispenserEx {

  }

}