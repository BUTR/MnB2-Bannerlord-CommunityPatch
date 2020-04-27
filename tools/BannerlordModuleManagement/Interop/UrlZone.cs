#pragma warning disable 1519
namespace BannerlordModuleManagement.Interop {

  /// <seealso href="https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms537175(v%3Dvs.85)"/>
  public enum UrlZone {

    /// <summary>Invalid zone.</summary>
    Invalid = -1,

    /// <summary>Local machine zone.</summary>
    LocalMachine = 0,

    /// <summary>Intranet zone.</summary>
    Intranet,

    /// <summary>Trusted zone.</summary>
    Trusted,

    /// <summary>Internet zone.</summary>
    Internet,

    /// <summary>Untrusted zone.</summary>
    Untrusted

  }

}