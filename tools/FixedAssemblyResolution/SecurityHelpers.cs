using System;
using System.IO;
using Com;

namespace FixedAssemblyResolution {

  public static class SecurityHelpers {

    public static void UnblockFile(string absPath) {
      var pzi = new PersistentZoneIdentifier();
      try {
        pzi.Load(absPath, StorageMode.Read | StorageMode.ShareExclusive);
      }
      catch (FileNotFoundException) {
        // not blocked
        return;
      }

      pzi.Remove();
      pzi.Save(absPath, false);
      
    }

  }

}