using System;
using System.Threading;


namespace Antijank {

  
  public class WarpContext : HostExecutionContext {

    public WarpContext()
      => _parent = Thread.CurrentThread;

    private readonly Thread _parent;

    private Thread _current;

    private WarpManager _manager;

    private int _reverted;

    [ThreadStatic]
    private static WarpContext _local;

    private bool _disposed;

    public void SetManager(WarpManager manager) {
      _local = this;
      _manager = manager;
      _current = Thread.CurrentThread;
    }

    public void Revert() {
      ++_reverted;
      _current = null;
    }

    public Thread Thread => _current;

    public override HostExecutionContext CreateCopy()
      => this;

    public override void Dispose(bool disposing) {
      _disposed = true;
      _current = null;

      _manager?.Remove(this);
      _manager = null;
    }

    public bool Disposed => _disposed;

    public static WarpContext Local
      => _local;

    public WarpManager Manager
      => _manager;

    public Thread Parent
      => _parent;

  }

}