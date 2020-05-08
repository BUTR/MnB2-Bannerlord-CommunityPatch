#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;

namespace Antijank.Debugging {

  public readonly struct FastTypeHandle {

    public static Assembly? GetAssembly(IntPtr address) {
      var addr = unchecked((ulong) address.ToInt64());
      string? modName = null;
      DebuggerContext.Current.Send(() => {
        modName = DebuggerContext.GetModuleAtAddress(addr);
      });
      if (modName == null)
        return null;

      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        var asmPath = Path.GetFullPath(new Uri(asm.CodeBase).LocalPath);
        if (asmPath.Equals(modName, StringComparison.OrdinalIgnoreCase))
          return asm;
      }

      return null;
    }

    public static IntPtr GetBaseAddressAndSize(Assembly asm, out UIntPtr size) {
      var asmPath = Path.GetFullPath(new Uri(asm.CodeBase).LocalPath);
      ulong address = 0;
      uint s = 0;
      DebuggerContext.Current.Send(() => {
        address = DebuggerContext.FindModuleAddressAndSize(asmPath, out s);
      });
      size = (UIntPtr) s;
      return (IntPtr) address;
    }

    public static IntPtr GetBaseAddress(Assembly asm)
      => GetBaseAddressAndSize(asm, out _);

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