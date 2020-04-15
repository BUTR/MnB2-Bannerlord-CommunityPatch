using System;
using TaleWorlds.Engine;

namespace CommunityPatch {

  public static class PathHelpers {

    private static string _binSubDir;

    public static string GetBinSubDir() {
      if (_binSubDir != null)
        return _binSubDir;

      try {
        _binSubDir = Utilities.GetConfigsPath();
      }
      catch (NullReferenceException) {
        _binSubDir = "Win64_Shipping_Client";
      }

      return _binSubDir;
    }

    private static string _configsDir;

    public static string GetConfigsDir() {
      if (_configsDir != null)
        return _configsDir;

      try {
        _configsDir = Utilities.GetConfigsPath();
      }
      catch (NullReferenceException) {
        _configsDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Configs");
      }

      return _configsDir;
    }

  }

}