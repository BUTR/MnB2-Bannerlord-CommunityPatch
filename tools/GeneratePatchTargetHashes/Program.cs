#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Path = System.IO.Path;

internal static class Program {

  private static readonly string AsmDir = Path.GetDirectoryName(new Uri(typeof(Program).Assembly.CodeBase!).LocalPath)!;

  private static readonly string ModBinDir = new Uri(Path.Combine(AsmDir, "..", "..", "..", "..", "..", "bin", "Win64_Shipping_Client")).LocalPath;

  private static readonly string GameRoot = new Uri(Path.Combine(AsmDir, "..", "..", "..", "..", "..", "..", "..")).LocalPath;

  private static readonly string ModulesRoot = new Uri(Path.Combine(GameRoot, "Modules")).LocalPath;

  private static readonly string NativeBinDir = new Uri(Path.Combine(ModulesRoot, "Native", "bin", "Win64_Shipping_Client")).LocalPath;

  private static readonly string SandBoxBinDir = new Uri(Path.Combine(ModulesRoot, "SandBox", "bin", "Win64_Shipping_Client")).LocalPath;

  private static readonly string StoryModeBinDir = new Uri(Path.Combine(ModulesRoot, "StoryMode", "bin", "Win64_Shipping_Client")).LocalPath;

  private static readonly string GameBinDir = new Uri(Path.Combine(GameRoot, "bin", "Win64_Shipping_Client")).LocalPath;

  private static void Main() {
    Environment.CurrentDirectory = GameBinDir;

    AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
    AppDomain.CurrentDomain.TypeResolve += TypeResolver;

    CommunityPatchLoader.GenerateHashes();
  }

  private static Assembly? AssemblyResolver(object sender, ResolveEventArgs eventArgs) {
    var name = eventArgs.Name;
    //Console.WriteLine("Assembly Requested: " + name);

    if (name == null)
      return null;

    var asmName = new AssemblyName(name).Name;

    var fileName = asmName;

    if (fileName == null)
      return null;

    if (!fileName.EndsWith(".dll"))
      fileName += ".dll";

    Assembly? TryLoadFromDir(string dir) {
      var path = Path.Combine(dir, fileName!);
      return File.Exists(path)
        ? Assembly.LoadFrom(path)
        : null;
    }

    var loadAttempt = TryLoadFromDir(ModBinDir)
      ?? TryLoadFromDir(GameBinDir)
      ?? TryLoadFromDir(NativeBinDir)
      ?? TryLoadFromDir(SandBoxBinDir)
      ?? TryLoadFromDir(StoryModeBinDir);

    if (loadAttempt != null)
      return loadAttempt;

    Console.WriteLine("Assembly Requested: " + name);
    Console.WriteLine("Didn't load assembly.");
    if (Debugger.IsAttached)
      Debugger.Break();

    return null;
  }

  private static Assembly? TypeResolver(object? sender, ResolveEventArgs args) {
    var name = args.Name;
    //Console.WriteLine($"Type Requested: {name}");
    if (name?.StartsWith("Mono") ?? false)
      return null;

    return null;
  }

}