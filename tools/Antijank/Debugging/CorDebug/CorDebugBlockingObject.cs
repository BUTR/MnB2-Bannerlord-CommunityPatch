using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public struct CorDebugBlockingObject {

    public ICorDebugValue pBlockingObject;

    public uint dwTimeout;

    public CorDebugBlockingReason blockingReason;

  }

}