using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using TaleWorlds.Engine;
using Path = System.IO.Path;

internal static class Program {

  private static void Main() {
    // ReSharper disable once AssignNullToNotNullAttribute
    var asmDir = Path.GetDirectoryName(new Uri(typeof(Program).Assembly.CodeBase).LocalPath);
    // ReSharper disable once AssignNullToNotNullAttribute
    var modBinDir = new Uri(Path.Combine(asmDir, "..", "..", "..", "..", "..", "bin", "Win64_Shipping_Client")).LocalPath;
    var gameRoot = new Uri(Path.Combine(asmDir, "..", "..", "..", "..", "..", "..", "..")).LocalPath;
    var modulesRoot = new Uri(Path.Combine(gameRoot, "Modules")).LocalPath;
    var nativeBinDir = new Uri(Path.Combine(modulesRoot, "Native", "bin", "Win64_Shipping_Client")).LocalPath;
    var sandBoxBinDir = new Uri(Path.Combine(modulesRoot, "SandBox", "bin", "Win64_Shipping_Client")).LocalPath;
    var gameBinDir = new Uri(Path.Combine(gameRoot, "bin", "Win64_Shipping_Client")).LocalPath;
    Environment.CurrentDirectory = gameBinDir;

    Assembly AssemblyResolver(object sender, ResolveEventArgs eventArgs) {
      //Console.WriteLine("Loading: " + eventArgs.Name);

      if (eventArgs.Name == null)
        return null;

      var name = new AssemblyName(eventArgs.Name);

      var fileName = name.Name;

      if (fileName == null)
        return null;

      if (!fileName.EndsWith(".dll"))
        fileName += ".dll";

      Assembly TryLoadFromDir(string dir) {
        var path = Path.Combine(dir, fileName);
        return File.Exists(path)
          ? Assembly.LoadFile(path)
          : null;
      }

      return TryLoadFromDir(modBinDir) ?? TryLoadFromDir(gameBinDir) ?? TryLoadFromDir(nativeBinDir) ?? TryLoadFromDir(sandBoxBinDir);
    }

    AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;

    CommunityPatchLoader.GenerateHashes();
  }

}