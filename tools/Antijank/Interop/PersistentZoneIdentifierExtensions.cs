

namespace Antijank.Interop {

  
  public static class PersistentZoneIdentifierExtensions {

    public static void Load(this PersistentZoneIdentifier pzi, string fileName, StorageMode storageMode)
      => pzi.Load(fileName, (int) storageMode);

  }

}