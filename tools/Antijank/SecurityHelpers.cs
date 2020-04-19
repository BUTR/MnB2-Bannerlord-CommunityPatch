using System.IO;
using Com;

namespace Antijank {

  public static class SecurityHelpers {

    public static bool UnblockFile(string absPath) {
      var pzi = new PersistentZoneIdentifier();
      try {
        pzi.Load(absPath, StorageMode.Read | StorageMode.ShareExclusive);
      }
      catch (FileNotFoundException) {
        // not blocked
        return false;
      }

      pzi.Remove();
      pzi.Save(absPath, false);

      return true;
    }

  }

}