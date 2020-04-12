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

  }

}