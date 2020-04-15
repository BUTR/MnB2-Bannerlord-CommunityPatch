using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.Engine;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    private static string Eol => Environment.NewLine;

    [PublicAPI]
    [Conditional("TRACE")]
    public static void Error(Exception ex, string msg = null) {
      if (msg != null)
        Error(msg);

      switch (ex) {
        case TargetInvocationException tie1:
          Error(tie1.InnerException, msg);
          return;
        case TypeInitializationException tie2:
          Error(tie2.InnerException, msg);
          return;
        case AggregateException aex:
          foreach (var iex in aex.InnerExceptions)
            Error(iex.InnerException, msg);
          return;
      }

      var st = new StackTrace(ex, true);
      var f = st.GetFrame(0);
      var exMsg = $"{f.GetFileName()}:{f.GetFileLineNumber()}:{f.GetFileColumnNumber()}: {ex.GetType().Name}: {ex.Message}";

      MBDebug.ConsolePrint(exMsg);
      MBDebug.ConsolePrint(ex.StackTrace);
      Debugger.Log(3, nameof(CommunityPatch), exMsg + Eol);
      Debugger.Log(3, nameof(CommunityPatch), ex.StackTrace + Eol);
    }

    [PublicAPI]
    [Conditional("TRACE")]
    public static void Error(Exception ex, FormattableString msg)
      => Error(ex, FormattableString.Invariant(msg));

    [PublicAPI]
    [Conditional("TRACE")]
    public static void Error(FormattableString msg)
      => Error(FormattableString.Invariant(msg));

    [PublicAPI]
    [Conditional("TRACE")]
    public static void Error(string msg = null) {
      if (msg == null)
        return;

      if (!msg.EndsWith("\n"))
        msg += Eol;
      Debugger.Log(3, nameof(CommunityPatch), msg);
    }

    [PublicAPI]
    [Conditional("DEBUG")]
    public static void Print(FormattableString msg)
      => Print(FormattableString.Invariant(msg));

    [PublicAPI]
    [Conditional("DEBUG")]
    public static void Print(string msg)
      => Debugger.Log(0, nameof(CommunityPatch), msg + Eol);

    internal static readonly LinkedList<Exception> RecordedFirstChanceExceptions
      = new LinkedList<Exception>();

    internal static readonly LinkedList<Exception> RecordedUnhandledExceptions
      = new LinkedList<Exception>();

  }

}