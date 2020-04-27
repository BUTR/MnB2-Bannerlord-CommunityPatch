using System.IO;
using BannerlordModuleManagement.Interop;

namespace BannerlordModuleManagement {

  /// <summary>
  /// Provides helpers to deal with security concerns.
  /// </summary>
  public static class SecurityHelpers {

    /// <summary>
    /// Unblocks a file by removing it's security zone information.
    /// </summary>
    /// <param name="absPath">A file's path.</param>
    /// <returns></returns>
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