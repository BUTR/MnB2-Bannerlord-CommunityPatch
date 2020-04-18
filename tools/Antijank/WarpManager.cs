using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace Antijank {

  [PublicAPI]
  public class WarpManager : HostExecutionContextManager {

    public static readonly WarpManager Instance = new WarpManager();

    private readonly Thread _root;

    private readonly LinkedList<WarpContext> _web
      = new LinkedList<WarpContext>();

    private WarpManager() {
      _root = Thread.CurrentThread;
    }

    public override HostExecutionContext Capture() {
      var ctx = new WarpContext();
      lock (_web)
        _web.AddLast(ctx);
      return ctx;
    }

    public override object SetHostExecutionContext(HostExecutionContext hostExecutionContext) {
      if (!(hostExecutionContext is WarpContext warp))
        return new WarpContext();

      warp.SetManager(this);
      return warp;
    }

    public override void Revert(object previousState) {
      if (previousState is WarpContext warp) {
        warp.Revert();
        return;
      }

      base.Revert(previousState);
    }

  }

}