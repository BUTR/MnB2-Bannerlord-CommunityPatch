using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  [ComImport, Guid("D0E80DD3-12D4-11d3-B39D-00C04FF81795"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IHostFilter {

    int MarkToken(uint tk);

  }

}