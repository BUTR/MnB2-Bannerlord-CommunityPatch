using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Sigil;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public static class CallStackHelpers {

    public const BindingFlags AnyAccess = Public | NonPublic;

    static CallStackHelpers() {
      var sfhType = typeof(object).Assembly.GetType("System.Diagnostics.StackFrameHelper");
      var dm = Emit<Func<Thread, int>>.NewDynamicMethod("GetThreadCallStackDepth", strictBranchVerification: true);

      var depth = dm.DeclareLocal(typeof(int));
      var sfh = dm.DeclareLocal(sfhType);
      dm.LoadArgument(0);
      dm.NewObject(sfhType, typeof(Thread));
      dm.StoreLocal(sfh);
      dm.BeginExceptionBlock(out var tryBlock);
      dm.LoadLocal(sfh);
      dm.LoadConstant(0);
      dm.LoadConstant(false);
      dm.LoadNull();
      dm.Call(typeof(StackTrace).GetMethod("GetStackFramesInternal", AnyAccess | Static | FlattenHierarchy));
      dm.LoadLocal(sfh);
      dm.Call(sfhType.GetMethod("GetNumberOfFrames", AnyAccess | Instance | FlattenHierarchy));
      dm.StoreLocal(depth);
      dm.BeginFinallyBlock(tryBlock, out var finallyBlock);
      dm.LoadLocal(sfh);
      dm.Call(typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose)));
      dm.EndFinallyBlock(finallyBlock);
      dm.EndExceptionBlock(tryBlock);
      dm.LoadLocal(depth);
      dm.Return();
      _GetThreadCallStackDepth = dm.CreateDelegate();
    }

    public static readonly Func<Thread, int> _GetThreadCallStackDepth;

    public static int GetThreadCallStackDepth(this Thread thread) {
      var prev = AssemblyResolver.DisableFirstChanceExceptionPrinting;
      AssemblyResolver.DisableFirstChanceExceptionPrinting = true;
      var depth = _GetThreadCallStackDepth(thread);
      AssemblyResolver.DisableFirstChanceExceptionPrinting = prev;
      return depth;
    }

    public static void Init() {
      // static constructor will init
    }

  }

}