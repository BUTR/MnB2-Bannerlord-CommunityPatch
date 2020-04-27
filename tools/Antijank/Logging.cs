using System;
using System.Reflection;

namespace Antijank {

  public static class Logging {

    public static void Log(Exception ex) {
      while (ex != null) {
        Console.WriteLine(ex.ToString());

        if (ex is ReflectionTypeLoadException rtl) {
          foreach (var lex in rtl.LoaderExceptions)
            Log(lex);
        }
        else if (ex is AggregateException aex) {
          foreach (var cex in aex.InnerExceptions)
            Log(cex);
          return;
        }

        ex = ex.InnerException;
      }
    }

  }

}