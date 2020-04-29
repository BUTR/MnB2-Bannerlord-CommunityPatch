using System.Runtime.InteropServices;


namespace Antijank.Interop {

  
  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CD45F185-1B21-48E2-967B-EAD743A8914E")]
  public interface IZoneIdentifier {

    UrlZone GetId();

    void SetId(UrlZone id);

    void Remove();

  }

}