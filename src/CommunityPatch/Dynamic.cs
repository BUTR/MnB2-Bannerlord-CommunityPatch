using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  public static class Dynamic {

    public static readonly AssemblyName AssemblyName = new AssemblyName("Community Patch Dynamic Assembly");

    public static readonly AssemblyBuilder Assembly = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndCollect);

    public static readonly ModuleBuilder MainModule = Assembly.DefineDynamicModule(AssemblyName.Name);

    private static readonly HashSet<string> InternalsAccessed = new HashSet<string>();

    private static long _unique = 0;

    public static TypeBuilder CreateStaticClass(string name = null)
      => MainModule.DefineType(name ?? $"StaticClass{Interlocked.Increment(ref _unique)}", TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public);

    public static void AccessInternals(MethodBase methodBase) {
      AccessInternals(methodBase.DeclaringType);
      AccessInternals(methodBase.ReflectedType);
      if (!(methodBase is MethodInfo mi))
        return;

      AccessInternals(mi.ReturnType);
      foreach (var p in mi.GetParameters())
        AccessInternals(p.ParameterType);
    }

    public static void AccessInternals(Type type)
      => AccessInternals(type.Assembly);

    private static void AccessInternals(Assembly asm) {
      lock (InternalsAccessed) {
        var asmName = asm.GetName().Name;
        if (InternalsAccessed.Add(asmName)) {
          Assembly.SetCustomAttribute(
            new CustomAttributeBuilder(
              IgnoresAccessChecksToAttribute.ConstructorInfo,
              new object[] {asmName}
            )
          );
        }
      }
    }

  }

}