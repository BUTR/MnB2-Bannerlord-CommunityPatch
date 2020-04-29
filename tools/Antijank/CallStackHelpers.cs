using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using Sigil;
using static System.Reflection.BindingFlags;
using ModuleInfo = TaleWorlds.Library.ModuleInfo;

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

    private static readonly Func<Thread, int> _GetThreadCallStackDepth;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int GetCallStackDepth(this Thread thread) {
      if (thread == Thread.CurrentThread)
        thread = null;
      var prev = Options.DisableFirstChanceExceptionPrinting;
      Options.DisableFirstChanceExceptionPrinting = true;
      var depth = _GetThreadCallStackDepth(thread);
      Options.DisableFirstChanceExceptionPrinting = prev;
      if (thread == null)
        --depth;
      return depth;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int GetCallStackDepth() {
      var prev = Options.DisableFirstChanceExceptionPrinting;
      Options.DisableFirstChanceExceptionPrinting = true;
      var depth = _GetThreadCallStackDepth(null);
      Options.DisableFirstChanceExceptionPrinting = prev;
      return --depth;
    }

    public static void Init() {
      // static constructor will init
    }

    public static Assembly FindModuleFromStackTrace(this StackTrace stackTrace) {
      var stackFrames = stackTrace.GetFrames();
      if (stackFrames == null) {
        return null;
      }

      foreach (var stackFrame in stackFrames) {
        var method = stackFrame.GetMethod();
        if (method == null)
          continue;

        var type = method.DeclaringType;
        if (type == null)
          continue;

        var asm = type.Assembly;

        if (PathHelpers.IsModuleAssembly(asm))
          return asm;
      }

      return null;
    }

    public static Assembly FindModuleFromStackTrace(this StackTrace stackTrace, out ModuleInfo modInfo) {
      var stackFrames = stackTrace.GetFrames();
      if (stackFrames == null) {
        modInfo = null;
        return null;
      }

      foreach (var stackFrame in stackFrames) {
        var method = stackFrame.GetMethod();
        if (method == null)
          continue;

        var type = method.DeclaringType;
        if (type == null)
          continue;

        var asm = type.Assembly;

        if (PathHelpers.IsModuleAssembly(asm, out modInfo))
          return asm;
      }

      modInfo = null;
      return null;
    }

    public static Assembly FindModuleFromStackTrace(this StackTrace stackTrace, out ModuleInfo modInfo, out StackFrame relevantFrame) {
      var stackFrames = stackTrace.GetFrames();
      if (stackFrames == null) {
        modInfo = null;
        relevantFrame = null;
        return null;
      }

      foreach (var stackFrame in stackFrames) {
        var method = stackFrame.GetMethod();
        if (method == null)
          continue;

        var type = method.DeclaringType;
        if (type == null)
          continue;

        var asm = type.Assembly;

        if (!PathHelpers.IsModuleAssembly(asm, out modInfo))
          continue;

        relevantFrame = stackFrame;
        return asm;
      }

      modInfo = null;
      relevantFrame = null;
      return null;
    }

    public static Exception UnnestCommonExceptions(Exception ex) {
      switch (ex) {
        case null:
          break;
        case TypeLoadException tle:
          ex = UnnestCommonExceptions(tle.InnerException);
          break;
        case AggregateException aex:
          ex = UnnestCommonExceptions(aex.InnerExceptions.FirstOrDefault()) ?? ex;
          break;
        case ReflectionTypeLoadException rtl:
          ex = UnnestCommonExceptions(rtl.LoaderExceptions.FirstOrDefault()) ?? ex;
          break;
        case TypeInitializationException tie:
          ex = UnnestCommonExceptions(tie.InnerException);
          break;
        case TargetInvocationException tie2:
          ex = UnnestCommonExceptions(tie2.InnerException);
          break;
      }

      return ex;
    }

  }

}