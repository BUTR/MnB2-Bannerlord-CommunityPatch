using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Module = TaleWorlds.MountAndBlade.Module;
using Path = System.IO.Path;

namespace Antijank {

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

    public static bool IsOfficialAssembly(this Assembly asm) {
      var path = new Uri(asm.CodeBase).LocalPath;
      return IsOfficialPath(path);
    }

    public static bool IsOfficialPath(string path) {
      // even if this isn't a dir, add a slash for matching purposes
      var dir = EnsureTrailingDirectorySeparator(path);

      var gameBinDir = GetGameBinDir();
      if (dir.StartsWith(gameBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var modsDir = GetModulesDir();

      var nativeBinDir = EnsureTrailingDirectorySeparator(Path.Combine(modsDir, "Native", "bin", GetBinSubDir()));
      if (dir.StartsWith(nativeBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbBinDir = EnsureTrailingDirectorySeparator(Path.Combine(modsDir, "SandBox", "bin", GetBinSubDir()));
      if (dir.StartsWith(sbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbcBinDir = EnsureTrailingDirectorySeparator(Path.Combine(modsDir, "SandBoxCore", "bin", GetBinSubDir()));
      if (dir.StartsWith(sbcBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var smBinDir = EnsureTrailingDirectorySeparator(Path.Combine(modsDir, "StoryMode", "bin", GetBinSubDir()));
      if (dir.StartsWith(smBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var cbBinDir = EnsureTrailingDirectorySeparator(Path.Combine(modsDir, "CustomBattle", "bin", GetBinSubDir()));
      if (dir.StartsWith(cbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      return false;
    }

    private static string _gameBaseDir;

    public static string GetGameBaseDir() {
      try {
        return _gameBaseDir ??= EnsureTrailingDirectorySeparator(Path.GetDirectoryName(Path.GetDirectoryName(GetGameBinDir().TrimEnd('\\', '/'))));
      }
      catch {
        return null;
      }
    }

    private static string _gameModsDir;

    public static string GetModulesDir() {
      try {
        return _gameModsDir ??= EnsureTrailingDirectorySeparator(Path.Combine(GetGameBaseDir(), "Modules"));
      }
      catch {
        return null;
      }
    }

    private static string _gameBinDir;

    public static string GetGameBinDir()
      => _gameBinDir ??= EnsureTrailingDirectorySeparator(Path.GetDirectoryName(new Uri(typeof(AssemblyResolver).Assembly.CodeBase).LocalPath));

    private static string EnsureTrailingDirectorySeparator(string path) {
      if (path[path.Length - 1] == Path.DirectorySeparatorChar)
        return path;

      return path + Path.DirectorySeparatorChar;
    }

    public static bool IsModuleAssembly(Assembly asm) {
      if (asm.IsDynamic)
        return false;

      var asmPath = new Uri(typeof(AssemblyResolver).Assembly.CodeBase).LocalPath;
      var modsDir = GetModulesDir();
      return asmPath.StartsWith(modsDir, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsModuleAssembly(Assembly asm, out ModuleInfo mod) {
      if (asm.IsDynamic) {
        mod = null;
        return false;
      }

      try {
        var asmPath = new Uri(asm.CodeBase).LocalPath;
        var modsDir = GetModulesDir();
        var isMod = asmPath.StartsWith(modsDir, StringComparison.OrdinalIgnoreCase);

        var modsSubDir = asmPath.Substring(modsDir.Length);
        var slashIndex = modsSubDir.IndexOf(Path.DirectorySeparatorChar);
        if (slashIndex == -1) {
          mod = null; // wtf?
          return true;
        }

        modsSubDir = modsSubDir.Substring(0, slashIndex);
        var mods = LoaderPatch.ModuleList ?? ModuleInfo.GetModules();
        mod = mods.FirstOrDefault(m => m.Alias == modsSubDir);

        return isMod;
      }
      catch {
        mod = null;
        return false;
      }
    }

    private static string _configsDir;

    public static string GetConfigsDir() {
      if (_configsDir != null)
        return _configsDir;

      try {
        _configsDir = Utilities.GetConfigsPath();
      }
      catch (NullReferenceException) {
        _configsDir = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "Mount and Blade II Bannerlord",
          "Configs"
        );
      }

      return _configsDir;
    }

  }

}