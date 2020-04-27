using JetBrains.Annotations;

namespace Antijank.Debugging {

  
  public enum COR_PRF_STATIC_TYPE {

    COR_PRF_FIELD_NOT_A_STATIC,

    COR_PRF_FIELD_APP_DOMAIN_STATIC,

    COR_PRF_FIELD_THREAD_STATIC,

    COR_PRF_FIELD_CONTEXT_STATIC = 4,

    COR_PRF_FIELD_RVA_STATIC = 8

  }

}