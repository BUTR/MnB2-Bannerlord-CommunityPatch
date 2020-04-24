using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [Guid("B81FF171-20F3-11D2-8DCC-00A0C9B00521")]
  [CoClass(typeof(TypeNameFactoryClass))]
  [ComImport]
  [PublicAPI]
  public interface TypeNameFactory : ITypeNameFactory {

  }

}