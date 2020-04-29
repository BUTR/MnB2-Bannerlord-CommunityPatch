using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BannerlordModuleManagement {

  /// <summary>
  /// Provides assembly resolution of <c>TaleWorlds</c> assemblies and <c>Facepunch.Steamworks</c> too.
  /// </summary>
  public static class TaleWorldAssemblyLoader {

    static TaleWorldAssemblyLoader()
      => AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
        var asmName = new AssemblyName(args.Name).Name;

        if (asmName.Equals("Facepunch.Steamworks", StringComparison.Ordinal)) {
          var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.Is64BitProcess
              ? "Facepunch.Steamworks.Win64.dll"
              : throw new PlatformNotSupportedException()
            : throw new PlatformNotSupportedException();

          var asmDir = Path.GetDirectoryName(new Uri(typeof(TaleWorldAssemblyLoader).Assembly.CodeBase).LocalPath)
            ?? Environment.CurrentDirectory;

          var fullPath = Path.Combine(asmDir, fileName);

          return Assembly.LoadFrom(fullPath);
        }

        // ReSharper disable once InvertIf
        if (asmName.StartsWith("TaleWorlds.")) {
          var path = SteamHelper.GetSteamAppPath(261550);
          var fullPath = Path.Combine(path, asmName + ".dll");
          return Assembly.LoadFrom(fullPath);
        }

        return null;
      };

    /// <summary>
    /// Registers a <see cref="ResolveEventHandler"/> for the <see cref="AppDomain.CurrentDomain"/> to resolve <c>TaleWorlds</c> and <c>Facepunch.Steamworks</c> assemblies.
    /// </summary>
    public static void Register() {
      // invoke static init
    }

  }

}