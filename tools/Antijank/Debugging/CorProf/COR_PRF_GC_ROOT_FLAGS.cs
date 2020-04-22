using JetBrains.Annotations;

namespace Antijank.Debugging {

  [PublicAPI]
  public enum COR_PRF_GC_ROOT_FLAGS {

    COR_PRF_GC_ROOT_PINNING = 1,

    COR_PRF_GC_ROOT_WEAKREF,

    COR_PRF_GC_ROOT_INTERIOR = 4,

    COR_PRF_GC_ROOT_REFCOUNTED = 8

  }

}