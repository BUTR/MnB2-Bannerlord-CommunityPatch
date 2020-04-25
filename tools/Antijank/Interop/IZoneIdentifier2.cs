using System.Runtime.InteropServices;

namespace Antijank.Interop {

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