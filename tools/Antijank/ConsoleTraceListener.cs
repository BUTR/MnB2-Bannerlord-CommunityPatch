using System;
using System.Diagnostics;

namespace Antijank {

  public class ConsoleTraceListener : TextWriterTraceListener
  {
    public ConsoleTraceListener()
      : base(Console.Out)
    {
    }

    public override void Close()
    {
    }

    public override void Flush() {
      try {
        base.Flush();
      }
      catch {
        // whatever
      }
    }

  }

}