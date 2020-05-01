#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Antijank.Debugging {

  public readonly struct FastTypeHandle {

    public static T WithProcessModule<T>(Assembly asm, Func<ProcessModule, T> procModAction) {
      var asmPath = Path.GetFullPath(new Uri(asm.CodeBase).LocalPath);
      using var proc = Process.GetCurrentProcess();
      foreach (ProcessModule procMod in proc.Modules) {
        var procModPath = Path.GetFullPath(new Uri(procMod.FileName).LocalPath);

        if (!asmPath.Equals(procModPath,
          RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal))
          continue;

        return procModAction(procMod);
      }

      return default!;
    }

    public static Assembly? GetAssembly(ProcessModule procMod) {
      var procModPath = Path.GetFullPath(new Uri(procMod.FileName).LocalPath);

      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        var asmPath = Path.GetFullPath(new Uri(asm.CodeBase).LocalPath);

        if (asmPath.Equals(procModPath,
          RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal))
          return asm;
      }

      return null;
    }

    public static T WithProcessModule<T>(IntPtr address, Func<ProcessModule, T> procModAction) {
      using var proc = Process.GetCurrentProcess();
      foreach (ProcessModule procMod in proc.Modules) {
        if (procMod.BaseAddress != address)
          continue;

        return procModAction(procMod);
      }

      return default!;
    }

    public static Assembly? GetAssembly(IntPtr address)
      => WithProcessModule(address, GetAssembly);

    public static IntPtr GetBaseAddress(Assembly asm)
      => WithProcessModule(asm, module => module.BaseAddress);

    public readonly IntPtr ModuleAddress;

    public readonly int MetadataToken;

    public FastTypeHandle(Type type)
      : this(type.Assembly, type.MetadataToken) {
    }

    public FastTypeHandle(Assembly asm, int mdToken)
      : this(GetBaseAddress(asm), mdToken) {
    }

    public FastTypeHandle(IntPtr moduleAddress, int mdToken)
      : this() {
      ModuleAddress = moduleAddress;

      MetadataToken = mdToken;
    }

    public Module? ResolveModule()
      => GetAssembly(ModuleAddress)?.ManifestModule;

    public Type? ResolveType() {
      var mod = ResolveModule();
      if (mod == null)
        return null;

      var type = mod.ResolveType(MetadataToken);
      return type;
    }

    public MemberInfo? ResolveMember() {
      var mod = ResolveModule();
      if (mod == null)
        return null;

      var member = mod.ResolveMember(MetadataToken);
      return member;
    }

    public string? ResolveString() {
      var mod = ResolveModule();
      if (mod == null)
        return null;

      var s = mod.ResolveString(MetadataToken);
      return s;
    }

  }

}