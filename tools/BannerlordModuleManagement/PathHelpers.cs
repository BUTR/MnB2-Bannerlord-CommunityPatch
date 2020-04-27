using System;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Path = System.IO.Path;

namespace BannerlordModuleManagement {

  /// <summary>
  /// Provides helpers to deal with paths.
  /// </summary>
  public static class PathHelpers {

    private static readonly string DirSeparator = new string(Path.DirectorySeparatorChar, 1);

    private static readonly string DoubleDirSeparator = new string(Path.DirectorySeparatorChar, 2);

    private static string _binSubDir;

    /// <summary>
    /// Returns "Win64_Shipping_Client" unless the configuration name is different.
    /// </summary>
    /// <returns>"Win64_Shipping_Client"</returns>
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

    /// <summary>
    /// Retrieves the full path to the game's configuration directory.
    /// </summary>
    /// <returns>A path.</returns>
    public static string GetConfigsDir() {
      if (_configsDir != null)
        return _configsDir;

      try {
        _configsDir = EnsureTrailingDirectorySeparatorAndNormalize(Utilities.GetConfigsPath());
      }
      catch (NullReferenceException) {
        _configsDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "Mount and Blade II Bannerlord",
          "Configs"
        ));
      }

      return _configsDir;
    }

    private static string EnsureTrailingDirectorySeparatorAndNormalize(string path) {
      var sb = new StringBuilder(path);

      NormalizeSeparators(sb);

      if (sb[sb.Length - 1] != Path.DirectorySeparatorChar)
        sb.Append(Path.DirectorySeparatorChar);

      return Path.IsPathRooted(path)
        ? new Uri(sb.ToString()).LocalPath
        : sb.ToString();
    }

    /// <summary>
    /// Determines if an assembly is from a well-known official module or in the game's executable directory.
    /// </summary>
    /// <param name="asm">An assembly.</param>
    /// <returns>Whether the assembly is in a well-known official location.</returns>
    public static bool IsOfficialAssembly(this Assembly asm) {
      var path = new Uri(asm.CodeBase).LocalPath;
      return IsOfficialPath(path);
    }

    private static void NormalizeSeparators(StringBuilder sb) {
      sb.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      var l = sb.Length;
      for (;;) {
        sb.Replace(DoubleDirSeparator, DirSeparator);
        var nl = sb.Length;
        if (nl == l)
          break;

        l = nl;
      }
    }

    /// <summary>
    /// Determines if a path is rooted in an well-known official module or in the game's executable directory.
    /// </summary>
    /// <param name="path">A path.</param>
    /// <returns>Whether the path is rooted in a well-known official location.</returns>
    public static bool IsOfficialPath(string path) {
      // even if this isn't a dir, add a slash for matching purposes
      var dir = EnsureTrailingDirectorySeparatorAndNormalize(path);

      var gameBinDir = GetGameBinDir();
      if (dir.StartsWith(gameBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var modsDir = GetModulesDir();

      var nativeBinDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(modsDir, "Native", "bin", GetBinSubDir()));
      if (dir.StartsWith(nativeBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbBinDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(modsDir, "SandBox", "bin", GetBinSubDir()));
      if (dir.StartsWith(sbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var sbcBinDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(modsDir, "SandBoxCore", "bin", GetBinSubDir()));
      if (dir.StartsWith(sbcBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var smBinDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(modsDir, "StoryMode", "bin", GetBinSubDir()));
      if (dir.StartsWith(smBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      var cbBinDir = EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(modsDir, "CustomBattle", "bin", GetBinSubDir()));
      if (dir.StartsWith(cbBinDir, StringComparison.OrdinalIgnoreCase))
        return true;

      return false;
    }

    private static string _gameBaseDir;

    /// <summary>
    /// Retrieves the full path to the game's main root directory.
    /// </summary>
    /// <returns>A path.</returns>
    public static string GetGameBaseDir() {
      try {
        return _gameBaseDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.GetDirectoryName(Path.GetDirectoryName(GetGameBinDir().TrimEnd('\\', '/'))));
      }
      catch {
        return null;
      }
    }

    private static string _gameModsDir;

    /// <summary>
    /// Retrieves the full path to the game's modules directory.
    /// </summary>
    /// <returns>A path.</returns>
    public static string GetModulesDir() {
      try {
        return _gameModsDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(GetGameBaseDir(), "Modules"));
      }
      catch {
        return null;
      }
    }

    private static string _gameBinDir;

    /// <summary>
    /// Retrieves the full path to the game's main executables directory.
    /// </summary>
    /// <returns>A path.</returns>
    public static string GetGameBinDir()
      => _gameBinDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.GetDirectoryName(
        new Uri(typeof(TaleWorlds.MountAndBlade.Launcher.LauncherUI).Assembly.CodeBase).LocalPath));

    /// <summary>
    /// Determines if an assembly originates from a module.
    /// </summary>
    /// <param name="asm">An assembly.</param>
    public static bool IsModuleAssembly(this Assembly asm) {
      if (asm.IsDynamic)
        return false;

      var asmPath = new Uri(asm.CodeBase).LocalPath;
      var modsDir = GetModulesDir();
      return asmPath.StartsWith(modsDir, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if an assembly originates from a module, and if so, which one.
    /// </summary>
    /// <param name="asm">An assembly.</param>
    /// <param name="mod">A mod.</param>
    /// <returns></returns>
    public static bool IsModuleAssembly(this Assembly asm, out ModuleInfo mod) {
      if (asm.IsDynamic) {
        mod = null;
        return false;
      }

      try {
        var asmPath = new Uri(asm.CodeBase).LocalPath;
        return IsModulePath(asmPath, out mod);
      }
      catch {
        mod = null;
        return false;
      }
    }

    /// <summary>
    /// Determines if a path is rooted in a module, and if so, which one.
    /// </summary>
    /// <param name="path">A path.</param>
    /// <param name="mod">A mod.</param>
    public static bool IsModulePath(string path, out ModuleInfo mod) {
      var modsDir = GetModulesDir();
      var isMod = path.StartsWith(modsDir, StringComparison.OrdinalIgnoreCase);

      if (path.Length <= modsDir.Length) {
        mod = null;
        return false;
      }

      var modsSubDir = path.Substring(modsDir.Length);
      var slashIndex = modsSubDir.IndexOf(Path.DirectorySeparatorChar);
      if (slashIndex == -1) {
        mod = null; // wtf?
        return true;
      }

      modsSubDir = modsSubDir.Substring(0, slashIndex);
      var mods = ModuleHelpers.ModuleList ?? ModuleInfo.GetModules();
      mod = mods.FirstOrDefault(m => m.Alias == modsSubDir);

      return isMod;
    }

  }

}