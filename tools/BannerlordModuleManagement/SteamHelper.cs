using Steamworks;

namespace BannerlordModuleManagement {

  /// <summary>
  /// Provides helpers to interact with steam.
  /// </summary>
  public static class SteamHelper {

    /// <summary>
    /// Returns a Steam App's installed path.
    /// </summary>
    /// <param name="appId">The Id of a steam application.</param>
    /// <returns>The path to the app.</returns>
    public static string GetSteamAppPath(uint appId) {
      if (!SteamClient.IsValid)
        SteamClient.Init(appId);
      var path = SteamApps.AppInstallDir(appId);
      return path;
    }

  }

}