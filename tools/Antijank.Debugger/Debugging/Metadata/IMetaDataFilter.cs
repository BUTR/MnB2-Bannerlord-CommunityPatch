using System.Runtime.InteropServices;


namespace Antijank.Debugging {

  
  [ComImport, Guid("D0E80DD1-12D4-11d3-B39D-00C04FF81795"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMetaDataFilter {

    int UnmarkAll();

    int MarkToken(uint tk);

    int IsTokenMarked(uint tk, out bool pIsMarked);

  }

}