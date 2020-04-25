using JetBrains.Annotations;

namespace Antijank.Debugging {

  // ReSharper disable once IdentifierTypo // yes it should writable
  [PublicAPI]
  public enum SetWriteableMetadataUpdateMode {

    LegacyCompatPolicy,

    AlwaysShowUpdates

  }

}