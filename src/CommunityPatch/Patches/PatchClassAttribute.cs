using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace CommunityPatch {
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public sealed class PatchClassAttribute: Attribute {
    public HarmonyMethod Info = new HarmonyMethod();

    public PatchClassAttribute(Type declaringType)
      => Info.declaringType = declaringType;
  }
}
