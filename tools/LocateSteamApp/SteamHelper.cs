using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Build.Utilities;
using Steamworks;

public static class SteamHelper {

  public static string GetSteamAppPath(TaskLoggingHelper taskLoggingHelper, uint appId) {
    try {
      if (!SteamClient.IsValid)
        SteamClient.Init(appId);
    }
    catch (Exception ex) {
      taskLoggingHelper.LogWarning(ex.ToString());
#if DEBUG
      Debugger.Launch();
#endif
    }

    string path = null;
    try {
      path = SteamApps.AppInstallDir(appId);
    }
    catch (Exception ex) {
      taskLoggingHelper.LogError(ex.ToString());
#if DEBUG
      Debugger.Launch();
#endif
    }

    return path;
  }

}