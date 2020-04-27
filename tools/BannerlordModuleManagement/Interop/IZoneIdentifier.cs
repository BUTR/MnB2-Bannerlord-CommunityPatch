#pragma warning disable 1519
using System.Runtime.InteropServices;

namespace BannerlordModuleManagement.Interop {

  
  /// <seealso href="https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms537032(v%3Dvs.85)"/>
  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CD45F185-1B21-48E2-967B-EAD743A8914E")]
  public interface IZoneIdentifier {

    UrlZone GetId();

    void SetId(UrlZone id);

    void Remove();

  }

}