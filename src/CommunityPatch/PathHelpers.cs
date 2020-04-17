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
      var dir = EnsureTrailingDirectorySeparator(Path.GetDirectoryName(path));

      var basePath = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));
      try {
        basePath = new Uri(Path.Combine(Environment.CurrentDirectory, Utilities.GetBasePath())).LocalPath;
      }
      catch {
        // ok...
      }

      var gameBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "bin", Common.ConfigName));
      if (dir.Equals(gameBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var nativeBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "Modules", "Native", "bin", Common.ConfigName));
      if (dir.Equals(nativeBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "Modules", "SandBox", "bin", Common.ConfigName));
      if (dir.Equals(sbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbcBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "Modules", "SandBoxCore", "bin", Common.ConfigName));
      if (dir.Equals(sbcBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var smBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "Modules", "StoryMode", "bin", Common.ConfigName));
      if (dir.Equals(smBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var cbBinDir = EnsureTrailingDirectorySeparator(Path.Combine(basePath, "Modules", "CustomBattle", "bin", Common.ConfigName));
      if (dir.Equals(cbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      return false;
    }

    private static string EnsureTrailingDirectorySeparator(string path) {
      if (path[path.Length - 1] == Path.DirectorySeparatorChar)
        return path;

      return path + Path.DirectorySeparatorChar;
    }

  }

}