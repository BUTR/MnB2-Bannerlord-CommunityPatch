using System;
using System.Diagnostics;
using System.Threading;
using Sigil;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  public static class CallStackHelpers {

    public static readonly Func<int> GetCallStackDepth;

    static CallStackHelpers() {
      var stackFrameHelperType = typeof(object).Assembly.GetType("System.Diagnostics.StackFrameHelper");

      var getStackFramesInternal = Type.GetType("System.Diagnostics.StackTrace, mscorlib")
        ?.GetMethod("GetStackFramesInternal", Static | NonPublic);

      if (getStackFramesInternal == null)
        throw new PlatformNotSupportedException();

      var iFrameCountField = stackFrameHelperType
        .GetField("iFrameCount", NonPublic | Instance | DeclaredOnly);

      if (iFrameCountField == null)
        throw new PlatformNotSupportedException();

      var dm = Emit<Func<int>>.NewDynamicMethod(typeof(StackTrace), "GetStackFrameHelper");

      dm.DeclareLocal(stackFrameHelperType);

      var constructorInfo = stackFrameHelperType.GetConstructor(new[] {typeof(Thread)});

      if (constructorInfo == null)
        throw new PlatformNotSupportedException();

      dm.LoadNull();
      dm.NewObject(constructorInfo);
      var local0 = dm.DeclareLocal(stackFrameHelperType);
      dm.StoreLocal(local0);
      dm.LoadLocal(local0);
      dm.LoadConstant(0);

      dm.LoadConstant(0); // Extra parameter

      dm.LoadNull();
      dm.Call(getStackFramesInternal);
      dm.LoadLocal(local0);
      dm.LoadField(iFrameCountField);
      dm.Return();

      GetCallStackDepth = dm.CreateDelegate();
    }

    public static void Init() {
      // static constructor will init
    }

  }

}