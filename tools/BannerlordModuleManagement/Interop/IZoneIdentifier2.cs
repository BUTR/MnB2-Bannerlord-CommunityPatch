#pragma warning disable 1519
using System.Runtime.InteropServices;

namespace BannerlordModuleManagement.Interop {

  /// <seealso href="https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/mt243886(v=vs.85)"/>
  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CD45F185-1B21-48E2-967B-EAD743A8914E")]
  public interface IZoneIdentifier2 {

    string GetLastWriterPackageFamilyName();

    void SetLastWriterPackageFamilyName(string packageFamilyName);

    void RemoveLastWriterPackageFamilyName();

    uint GetAppZoneId();

    void SetAppZoneId(uint zone);

    void RemoveAppZoneId();

  }

}