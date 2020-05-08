#pragma warning disable 1591

namespace BannerlordModuleManagement.Interop {

  public static class PersistentZoneIdentifierExtensions {

    /// <summary>
    /// Provides a typed overload for the <see cref="PersistentZoneIdentifier.Load"/> method.
    /// </summary>
    public static void Load(this PersistentZoneIdentifier pzi, string fileName, StorageMode storageMode)
      => pzi.Load(fileName, (int) storageMode);

  }

}