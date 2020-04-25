using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum COR_PRF_GC_ROOT_KIND {

    COR_PRF_GC_ROOT_STACK = 1,

    COR_PRF_GC_ROOT_FINALIZER,

    COR_PRF_GC_ROOT_HANDLE,

    COR_PRF_GC_ROOT_OTHER = 0

  }

}