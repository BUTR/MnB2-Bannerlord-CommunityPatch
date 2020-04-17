using System;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Path = System.IO.Path;

namespace CommunityPatch {

  public static class PathHelpers {

    private static string _binSubDir;

    public static string GetBinSubDir() {
      if (_binSubDir != null)
        return _binSubDir;

      try {
        _binSubDir = Common.ConfigName;
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
        _configsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mount and Blade II Bannerlord", "Configs");
      }

      return _configsDir;
    }

    public static bool IsOfficialAssembly(Assembly asm) {
      var path = new Uri(asm.CodeBase).LocalPath;
      var dir = Path.GetDirectoryName(path);

      var basePath = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));
      try {
        basePath = Utilities.GetBasePath();
      }
      catch {
        // ok...
      }

      var gameBinDir = Path.Combine(basePath, "bin", Common.ConfigName);
      if (dir.Equals(gameBinDir))
        return true;

      var nativeBinDir = Path.Combine(basePath, "Modules", "Native", "bin", Common.ConfigName);
      if (dir.Equals(nativeBinDir))
        return true;

      var sbBinDir = Path.Combine(basePath, "Modules", "SandBox", "bin", Common.ConfigName);
      if (dir.Equals(sbBinDir))
        return true;

      var sbcBinDir = Path.Combine(basePath, "Modules", "SandBoxCore", "bin", Common.ConfigName);
      if (dir.Equals(sbcBinDir))
        return true;

      var smBinDir = Path.Combine(basePath, "Modules", "StoryMode", "bin", Common.ConfigName);
      if (dir.Equals(smBinDir))
        return true;

      var cbBinDir = Path.Combine(basePath, "Modules", "CustomBattle", "bin", Common.ConfigName);
      if (dir.Equals(cbBinDir))
        return true;

      return false;
    }

  }

}