using System;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using Path = System.IO.Path;

namespace Antijank {

  public static class PathHelpers {

    private static readonly string DirSeparator = new string(Path.DirectorySeparatorChar, 1);

    private static readonly string DoubleDirSeparator = new string(Path.DirectorySeparatorChar, 2);

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

    public static string Normalize(string path) {
      var sb = new StringBuilder(path);

      NormalizeSeparators(sb);

      return Path.IsPathRooted(path)
        ? new Uri(sb.ToString()).LocalPath
        : sb.ToString();
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

    public static string GetGameBaseDir() {
      try {
        return _gameBaseDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.GetDirectoryName(Path.GetDirectoryName(GetGameBinDir().TrimEnd('\\', '/'))));
      }
      catch {
        return null;
      }
    }

    private static string _gameModsDir;

    public static string GetModulesDir() {
      try {
        return _gameModsDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.Combine(GetGameBaseDir(), "Modules"));
      }
      catch {
        return null;
      }
    }

    private static string _gameBinDir;

    public static string GetGameBinDir()
      => _gameBinDir ??= EnsureTrailingDirectorySeparatorAndNormalize(Path.GetDirectoryName(new Uri(typeof(AssemblyResolver).Assembly.CodeBase).LocalPath));

    public static bool IsModuleAssembly(Assembly asm) {
      if (asm.IsDynamic)
        return false;

      var asmPath = new Uri(asm.CodeBase).LocalPath;
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
        return IsModulePath(asmPath, out mod);
      }
      catch {
        mod = null;
        return false;
      }
    }

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
      var mods = ModuleInfo.GetModules();
      mod = mods.FirstOrDefault(m => m.Alias == modsSubDir);

      return isMod;
    }

  }

}