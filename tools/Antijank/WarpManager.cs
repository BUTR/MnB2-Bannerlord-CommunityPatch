using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Antijank {

  
  public class WarpManager : HostExecutionContextManager {

    public static readonly WarpManager Instance = new WarpManager();

    private readonly Thread _root;

    private readonly LinkedList<WarpContext> _web
      = new LinkedList<WarpContext>();

    
    public WarpContext FindContext(int id) {
      lock (_web)
        return _web.FirstOrDefault(ctx => ctx?.Thread?.ManagedThreadId == id);
    }

    private WarpManager() {
      _root = Thread.CurrentThread;
      var ctx = new WarpContext();
      _web.AddFirst(ctx);
      ctx.SetManager(this);
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

    internal void Remove(WarpContext warpContext) {
      lock (_web) {
        var link = _web.FindLast(warpContext);
        if (link != null)
          _web.Remove(link);
      }
    }

  }

}