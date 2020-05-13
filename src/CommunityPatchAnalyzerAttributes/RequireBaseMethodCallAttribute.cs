using System;

namespace CommunityPatchAnalyzer {

  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public sealed class RequireBaseMethodCallAttribute : Attribute {

    public RequireBaseMethodCallAttribute() {
    }

  }

}