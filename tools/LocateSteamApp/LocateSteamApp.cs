using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Steamworks;

[PublicAPI]
public sealed class LocateSteamAppTask : Task {

  static LocateSteamAppTask()
    => AppDomain.CurrentDomain.AssemblyResolve
      += (sender, args)
        => {
        var asmName = new AssemblyName(args.Name).Name;

        if (!asmName.Equals("Facepunch.Steamworks", StringComparison.Ordinal))
          return null;

#if DEBUG
        Debugger.Launch();
#endif

        var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
          ? Environment.Is64BitProcess
            ? "Facepunch.Steamworks.Win64.dll"
            : "Facepunch.Steamworks.Win32.dll"
          : "Facepunch.Steamworks.Posix.dll";

        var asmDir = System.IO.Path.GetDirectoryName(new Uri(typeof(LocateSteamAppTask).Assembly.CodeBase).LocalPath)
          ?? Environment.CurrentDirectory;

        var fullPath = System.IO.Path.Combine(asmDir, fileName);

        return Assembly.LoadFile(fullPath);
      };

  [Required]
  public uint AppId { get; set; }

  [Output]
  public string Path { get; set; }

  public override bool Execute() {
    var appId = AppId;

    var path = SteamHelper.GetSteamAppPath(Log, appId);

    Path = path;
    if (Path == null)
      return false;

    return true;
  }

}