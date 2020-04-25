using JetBrains.Annotations;

namespace Antijank.Interop {

  [PublicAPI]
  public enum UrlZone {

    Invalid = -1,

    LocalMachine = 0,

    Intranet,

    Trusted,

    Internet,

    Untrusted

  }

}