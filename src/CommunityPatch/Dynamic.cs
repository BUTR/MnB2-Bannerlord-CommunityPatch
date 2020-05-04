using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CommunityPatch {

  public static class Dynamic {

    public static readonly AssemblyName AssemblyName = new AssemblyName("Community Patch Dynamic Assembly");

    public static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndCollect);

    public static readonly ModuleBuilder MainModule = Assembly.DefineDynamicModule(AssemblyName.Name);

    private static long _unique = 0;

    public static TypeBuilder CreateStaticClass(string name = null)
      => MainModule.DefineType(name ?? $"StaticClass{Interlocked.Increment(ref _unique)}", TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public);

  }

}